using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Repos;
using Whisper.Chunks;
using ProtocolBuffers;
using Whisper.Messages;

namespace Whisper.ChunkGenerator
{
	/// <summary>
	/// Collection of chunks into a larger stream(file).
	/// </summary>
	public partial class StreamChunk
	{
		public StreamChunk ()
		{
			Chunks = new List<TrippleID> ();
		}

		/// <summary>
		/// Read file at path, split the contents in chunks and store them together with a StreamChunk.
		/// </summary>
		public static TrippleID GenerateChunk (string path, Repo repo)
		{
			StreamChunk message = new StreamChunk ();

			using (Stream stream = new FileStream(path, FileMode.Open)) {
				BinaryReader br = new BinaryReader (stream);

				message.Size = (ulong)stream.Length;

				while (true) {
					byte[] data = br.ReadBytes (4096);
					if (data.Length == 0)
						break;

					Chunk c = new Chunk (data);
					ChunkHash ch = repo.WriteChunk (c);

					message.Chunks.Add (new TrippleID (c.ClearHash, ch, c.CustomID));
				}
			}

			byte[] messageBytes = StreamChunk.SerializeToBytes (message);
			Chunk messageChunk = new Chunk (messageBytes);
			ChunkHash messageHash = repo.WriteChunk (messageChunk);

			return new TrippleID (messageChunk.ClearHash, messageHash, messageChunk.CustomID);
		}

		public static void Extract (Repo store, TrippleID fileCID, string targetPath)
		{
			Chunk chunk = store.ReadChunk (fileCID.ChunkHash);
			if (chunk.Verify (fileCID) == false)
				throw new InvalidDataException ("ClearID verification failed");
			StreamChunk streamChunk = StreamChunk.Deserialize<StreamChunk> (chunk.Data);
			
			using (FileStream file = File.Open(targetPath, FileMode.Create)) {
				foreach (TrippleID cid in streamChunk.Chunks) {
					Chunk fc = store.ReadChunk (cid.ChunkHash);
					if (fc.Verify (cid) == false)
						throw new InvalidDataException ("ClearID verification failed");
					
					file.Write (fc.Data, 0, fc.Data.Length);
				}
				
				//Verify length
				if (file.Length != (long)streamChunk.Size)
					throw new InvalidDataException ("Invalid file length");
			}
		}
	}
}

