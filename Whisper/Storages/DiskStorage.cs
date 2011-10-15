using System;
using System.IO;
using Whisper.Chunks;
using Whisper.Storages;
using System.Collections.Generic;
using Whisper.Messages;
using System.Text;
using ProtocolBuffers;

namespace Whisper.Storages
{
	/// <summary>
	/// Store chunks on lokal filesystem
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

		public override string ToString ()
		{
			return root;
		}

		public override ChunkHash GetCustomHash(CustomID id)
		{
			string idPath = Path.Combine(idRoot, id.ToString() + ".id");
			if (File.Exists(idPath) == false)
				return null;
			byte[] hash = File.ReadAllBytes(idPath);
			return ChunkHash.FromHashBytes(hash);
		}

		private string GetPath(ChunkHash hash)
		{
			string hex = hash.ToHex();
#if DEBUG
			if(hex.Length != 64) //32 bytes
				throw new InvalidDataException();
#endif
			string path = dataRoot;
			//FAT32 has a file per folder limit of slightly below 16 bits
			//Total 32 bytes = 64 characters hex
			path = Path.Combine(path, hex.Substring(0, 2));
			path = Path.Combine(path, hex.Substring(0, 4));
			path = Path.Combine(path, hex.Substring(0, 6));
			path = Path.Combine(path, hex);
			return path;
		}

		public override void WriteChunk(Chunk chunk)
		{
			//Data
			string dataPath = GetPath(chunk.ChunkHash);
			Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
			File.WriteAllBytes(dataPath, chunk.Data);

			//Keys
			if (chunk.Keys != null)
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Create))
				{
					ChunkKeys.Serialize(stream, chunk.Keys);
				}
			}

			//ID
			if (chunk.CustomID != null)
			{
				string idPath = Path.Combine(idRoot, chunk.CustomID.ToString() + ".id");
				File.WriteAllBytes(idPath, chunk.ChunkHash.bytes);
			}
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			Chunk chunk = new Chunk();
			
			//Read Data
			string dataPath = GetPath(chunkHash);
			chunk.Data = File.ReadAllBytes(dataPath);
			//Verify Hash
			chunk.ChunkHash = ChunkHash.FromHashBytes(Hash.ComputeHash(chunk.Data).bytes);
			if (chunk.ChunkHash.Equals(chunkHash) == false)
				throw new InvalidDataException("Hash mismatch: " + chunkHash);
			
			//Read keys
			string keyPath = dataPath + ".keys";
			if (File.Exists(keyPath))
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Open))
				{
					chunk.Keys = ChunkKeys.Deserialize<ChunkKeys>(stream);
				}
			}
			else
				chunk.ClearHash = ClearHash.FromHashBytes(chunkHash.bytes);

			return chunk;
		}

		public override void StoreMessage(ChunkHash id)
		{
			string path = Path.Combine(messageRoot, id.ToString());
			File.WriteAllBytes(path, new byte[0]);
		}

		public override List<ChunkHash> GetMessageList()
		{
			List<ChunkHash > list = new List<ChunkHash>();
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

