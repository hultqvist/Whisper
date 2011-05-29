using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Chunks;

namespace Whisper.Chunks
{
	/// <summary>
	/// Collection of blobs into a larger stream(file).
	/// </summary>
	public class StreamChunk : BinaryChunk
	{
		public long Size { get; set; }
		/// <summary>
		/// List of Blobs in order which constitutes the stream
		/// </summary>
		public List<TrippleID> Blobs = new List<TrippleID>();

		public static Chunk GenerateBlob(string path, Storage storage, ICollection<ChunkHash> blobList)
		{
			StreamChunk message = new StreamChunk();
			
			using (Stream stream = new FileStream(path, FileMode.Open))
			{
				BinaryReader br = new BinaryReader(stream);
				
				message.Size = stream.Length;
				
				while (true)
				{
					byte[] data = br.ReadBytes(4096);
					if (data.Length == 0)
						break;
					
					Chunk c = new Chunk(data);
					storage.WriteChunk(c);
					
					message.Blobs.Add(c.ClearID);
				}
			}

			Chunk messageBlob = message.ToBlob();
			storage.WriteChunk(messageBlob);
			
			if (blobList != null)
			{
				foreach (TrippleID cid in message.Blobs)
					blobList.Add(cid.ChunkHash);
				blobList.Add(messageBlob.ChunkHash);
			}
			
			return messageBlob;
		}

		public static void Extract(Storage store, TrippleID fileCID, string targetPath)
		{
			Chunk fileBlob = store.ReadChunk(fileCID.ChunkHash);
			if (fileBlob.Verify(fileCID) == false)
				throw new InvalidDataException("ClearID verification failed");
			StreamChunk streamBlob = StreamChunk.FromBlob(fileBlob);
			
			using (FileStream file = File.Open(targetPath, FileMode.Create))
			{
				foreach (TrippleID cid in streamBlob.Blobs)
				{
					Chunk fc = store.ReadChunk(cid.ChunkHash);
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

		internal override void WriteChunk(BinaryWriter writer)
		{
			writer.Write((int) Size);
			writer.Write((int) Blobs.Count);
			foreach (TrippleID id in Blobs)
			{
				id.WriteChunk(writer);
			}
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			Size = reader.ReadInt32();
			int count = reader.ReadInt32();
			for (int n = 0; n < count; n++)
			{
				TrippleID id = TrippleID.FromBlob(reader);
				Blobs.Add(id);
			}
		}

		static internal StreamChunk FromBlob(Chunk blob)
		{
			StreamChunk stream = new StreamChunk();
			stream.ReadChunk(blob);
			return stream;
		}

		#endregion
		
	}
}

