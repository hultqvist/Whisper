using System;
using System.IO;
using Whisper.Blobs;
using Whisper.ID;
using System.Collections.Generic;
using Whisper.Messages;
using System.Text;

namespace Whisper.Storages
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

		public override BlobHash GetBlobHash(CustomID id)
		{
			string idPath = Path.Combine(idRoot, id.ToString() + ".id");
			if (File.Exists(idPath) == false)
				return null;
			byte[] hash = File.ReadAllBytes(idPath);
			return new BlobHash(new Hash(hash));
		}

		public override void WriteBlob(Blob blob)
		{
			//Data
			string dataPath = Path.Combine(dataRoot, blob.BlobHash.ToString());
			File.WriteAllBytes(dataPath, blob.Data);
			
			//Keys
			if (blob.Keys != null)
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Create))
				{
					BinaryWriter bw = new BinaryWriter(stream);
					blob.Keys.WriteBlob(bw);
				}
			}

			//ID
			if (blob.CustomID != null)
			{
				string idPath = Path.Combine(idRoot, blob.CustomID.ToString() + ".id");
				File.WriteAllBytes(idPath, blob.BlobHash.bytes);
			}
		}

		public override Blob ReadBlob(BlobHash blobHash)
		{
			Blob blob = new Blob();
			
			//Read Data
			string dataPath = Path.Combine(dataRoot, blobHash.ToString());
			blob.Data = File.ReadAllBytes(dataPath);
			//Verify Hash
			blob.BlobHash = new BlobHash(Hash.ComputeHash(blob.Data));
			if (blob.BlobHash.Equals(blobHash) == false)
				throw new InvalidDataException("Hash mismatch: " + blobHash);
			
			//Read keys
			string keyPath = dataPath + ".keys";
			if (File.Exists(keyPath))
			{
				using (FileStream stream = new FileStream(dataPath + ".keys", FileMode.Open))
				{
					BinaryReader br = new BinaryReader(stream);
					blob.Keys = new BlobKeys();
					blob.Keys.ReadBlob(br);
				}
			}
			else
				blob.ClearHash = new Hash(blobHash);

			return blob;
		}

		public override void StoreMessage(BlobHash id)
		{
			string path = Path.Combine(messageRoot, id.ToString());
			File.WriteAllBytes(path, new byte[0]);
		}

		public override List<BlobHash> GetMessageList()
		{
			List<BlobHash> list = new List<BlobHash>();
			string[] files = Directory.GetFiles(messageRoot);
			
			foreach (string file in files)
			{
				string name = Path.GetFileName(file);
				list.Add(BlobHash.FromString(name));
			}
			return list;
		}
		
	}
}

