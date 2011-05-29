using System;
using System.IO;
using Whisper.Chunks;
using Whisper.Storing;
using System.Collections.Generic;
using Whisper.Messaging;
using System.Text;

namespace Whisper.Storing
{
	/// <summary>
	/// Manage blob storage on disk.
	/// Can be local, removable devices or other network based file sharing services.
	/// </summary>
	public class DiskStorage : Storage
	{
		private readonly string root;
		private readonly string idRoot;
		private readonly string dataRoot;
		private readonly string messageRoot;

		public DiskStorage(string path)
		{
			root = path;
			idRoot = Path.Combine(path, "id");
			dataRoot = Path.Combine(path, "data");
			messageRoot = Path.Combine(path, "message");
			Directory.CreateDirectory(root);
			Directory.CreateDirectory(idRoot);
			Directory.CreateDirectory(dataRoot);
			Directory.CreateDirectory(messageRoot);
		}

		public override ChunkHash GetCustomHash(CustomID id)
		{
			string idPath = Path.Combine(idRoot, id.ToString() + ".id");
			if (File.Exists(idPath) == false)
				return null;
			byte[] hash = File.ReadAllBytes(idPath);
			return new ChunkHash(new Hash(hash));
		}

		public override void WriteChunk(Chunk blob)
		{
			//Data
			string dataPath = Path.Combine(dataRoot, blob.ChunkHash.ToString());
			File.WriteAllBytes(dataPath, blob.Data);
			
			//Keys
			if (blob.Keys != null)
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Create))
				{
					BinaryWriter bw = new BinaryWriter(stream);
					blob.Keys.WriteChunk(bw);
				}
			}

			//ID
			if (blob.CustomID != null)
			{
				string idPath = Path.Combine(idRoot, blob.CustomID.ToString() + ".id");
				File.WriteAllBytes(idPath, blob.ChunkHash.bytes);
			}
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			Chunk chunk = new Chunk();
			
			//Read Data
			string dataPath = Path.Combine(dataRoot, chunkHash.ToString());
			chunk.Data = File.ReadAllBytes(dataPath);
			//Verify Hash
			chunk.ChunkHash = new ChunkHash(Hash.ComputeHash(chunk.Data));
			if (chunk.ChunkHash.Equals(chunkHash) == false)
				throw new InvalidDataException("Hash mismatch: " + chunkHash);
			
			//Read keys
			string keyPath = dataPath + ".keys";
			if (File.Exists(keyPath))
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Open))
				{
					BinaryReader br = new BinaryReader(stream);
					chunk.Keys = new ChunkKeys();
					chunk.Keys.ReadChunk(br);
				}
			}
			else
				chunk.ClearHash = new Hash(chunkHash);

			return chunk;
		}

		public override void StoreMessage(ChunkHash id)
		{
			string path = Path.Combine(messageRoot, id.ToString());
			File.WriteAllBytes(path, new byte[0]);
		}

		public override ICollection<ChunkHash> GetMessageList()
		{
			List<ChunkHash> list = new List<ChunkHash>();
			string[] files = Directory.GetFiles(messageRoot);
			
			foreach (string file in files)
			{
				string name = Path.GetFileName(file);
				list.Add(ChunkHash.FromString(name));
			}
			return list;
		}
		
	}
}

