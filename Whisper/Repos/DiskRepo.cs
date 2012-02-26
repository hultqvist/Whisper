using System;
using System.IO;
using Whisper.Chunks;
using Whisper.Repos;
using System.Collections.Generic;
using Whisper.Messages;
using System.Text;
using ProtocolBuffers;

namespace Whisper.Repos
{
	/// <summary>
	/// Store chunks on lokal filesystem
	/// </summary>
	public class DiskRepo : Repo
	{
		private readonly string root;
		private readonly string idRoot;
		private readonly string dataRoot;
		private readonly string messageRoot;

		public DiskRepo (string path)
		{
			root = path;
			idRoot = Path.Combine (path, "id");
			dataRoot = Path.Combine (path, "data");
			messageRoot = Path.Combine (path, "message");
			Directory.CreateDirectory (root);
			Directory.CreateDirectory (idRoot);
			Directory.CreateDirectory (dataRoot);
			Directory.CreateDirectory (messageRoot);
		}

		public override string ToString ()
		{
			return root;
		}

		public override ChunkHash GetCustomHash (CustomID id)
		{
			string idPath = Path.Combine (idRoot, id.ToString () + ".id");
			if (File.Exists (idPath) == false)
				return null;
			byte[] hash = File.ReadAllBytes (idPath);
			return ChunkHash.FromHashBytes (hash);
		}

		private string GetPath (ChunkHash hash)
		{
			string hex = hash.ToHex ();
#if DEBUG
			if(hex.Length != 64) //32 bytes
				throw new InvalidDataException();
#endif
			string path = dataRoot;
			//FAT32 has a file per folder limit of slightly below 16 bits
			//Total 32 bytes = 64 characters hex
			path = Path.Combine (path, hex.Substring (0, 2));
			path = Path.Combine (path, hex.Substring (0, 4));
			path = Path.Combine (path, hex.Substring (0, 6));
			path = Path.Combine (path, hex);
			return path;
		}

		public override ChunkHash WriteChunk (Chunk chunk)
		{
			//Data
			string dataPath = GetPath (chunk.ChunkHash);
			Directory.CreateDirectory (Path.GetDirectoryName (dataPath));
			File.WriteAllBytes (dataPath, chunk.Data);

			//ID
			if (chunk.CustomID != null) {
				string idPath = Path.Combine (idRoot, chunk.CustomID.ToString () + ".id");
				File.WriteAllBytes (idPath, chunk.ChunkHash.bytes);
			}

			return chunk.ChunkHash;
		}

		public override Chunk ReadChunk (ChunkHash chunkHash)
		{
			//Read Data
			string dataPath = GetPath (chunkHash);
			Chunk chunk = new Chunk (File.ReadAllBytes (dataPath));

			//Verify Hash
			if (chunk.ChunkHash.Equals (chunkHash) == false)
				throw new InvalidDataException ("Hash mismatch: " + chunkHash);

			//Read keys
			chunk.ClearHash = ClearHash.FromHashBytes (chunkHash.bytes);

			return chunk;
		}

		public override void StoreMessage (string prefix, ChunkHash id)
		{
			string path = Path.Combine (messageRoot, Path.GetFileName (prefix));
			Directory.CreateDirectory(path);
			path = Path.Combine (path, id.ToString ());
			File.WriteAllBytes (path, new byte[0]);
		}

		public override List<ChunkHash> GetMessageList (string prefix)
		{
			string path = Path.Combine (messageRoot, Path.GetFileName (prefix));

			List<ChunkHash > list = new List<ChunkHash> ();
			string[] files = Directory.GetFiles (path);

			foreach (string file in files) {
				string name = Path.GetFileName (file);
				list.Add (ChunkHash.FromString (name));
			}
			return list;
		}
		
	}
}

