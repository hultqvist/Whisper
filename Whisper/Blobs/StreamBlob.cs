using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Storages;
using Whisper.Blobs;

namespace Whisper.Blobs
{
	/// <summary>
	/// Collection of blobs into a larger stream(file).
	/// </summary>
	public class StreamBlob : BinaryBlob
	{
		public long Size { get; set; }
		/// <summary>
		/// List of Blobs in order which constitutes the stream
		/// </summary>
		public List<ClearID> Blobs = new List<ClearID>();

		public static Blob GenerateBlob(string path, Storage storage, List<BlobHash> blobList)
		{
			StreamBlob message = new StreamBlob();
			
			using (Stream stream = new FileStream(path, FileMode.Open))
			{
				BinaryReader br = new BinaryReader(stream);
				
				message.Size = stream.Length;
				
				while (true)
				{
					byte[] data = br.ReadBytes(4096);
					if (data.Length == 0)
						break;
					
					Blob c = new Blob(data);
					storage.WriteBlob(c);
					
					message.Blobs.Add(c.ClearID);
				}
			}

			Blob messageBlob = message.ToBlob();
			storage.WriteBlob(messageBlob);
			
			if (blobList != null)
			{
				foreach (ClearID cid in message.Blobs)
					blobList.Add(cid.BlobHash);
				blobList.Add(messageBlob.BlobHash);
			}
			
			return messageBlob;
		}

		public static void Extract(Storage store, ClearID fileCID, string targetPath)
		{
			Blob fileBlob = store.ReadBlob(fileCID.BlobHash);
			if (fileBlob.Verify(fileCID) == false)
				throw new InvalidDataException("ClearID verification failed");
			StreamBlob streamBlob = StreamBlob.FromBlob(fileBlob);
			
			using (FileStream file = File.Open(targetPath, FileMode.Create))
			{
				foreach (ClearID cid in streamBlob.Blobs)
				{
					Blob fc = store.ReadBlob(cid.BlobHash);
					if (fc.Verify(cid) == false)
						throw new InvalidDataException("ClearID verification failed");
					
					file.Write(fc.Data, 0, fc.Data.Length);
				}
				
				//Verify length
				if (file.Length != streamBlob.Size)
					throw new InvalidDataException("Invalid file length");
			}
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			writer.Write((int) Size);
			writer.Write((int) Blobs.Count);
			foreach (ClearID id in Blobs)
			{
				id.WriteBlob(writer);
			}
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			Size = reader.ReadInt32();
			int count = reader.ReadInt32();
			for (int n = 0; n < count; n++)
			{
				ClearID id = ClearID.FromBlob(reader);
				Blobs.Add(id);
			}
		}

		static internal StreamBlob FromBlob(Blob blob)
		{
			StreamBlob stream = new StreamBlob();
			stream.ReadBlob(blob);
			return stream;
		}

		#endregion
		
	}
}

