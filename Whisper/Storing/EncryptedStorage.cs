using System;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Blobing;
using System.IO;
using System.Security.Cryptography;

namespace Whisper.Storing
{
	/// <summary>
	/// Encrypt all data before being stored on backend
	/// </summary>
	public class EncryptedStorage : StorageFilter
	{
		private readonly IGenerateID idGenerator;
		/// <summary>
		/// Recipient keys or, when reading, all keys that might be used in decoding a message
		/// </summary>
		public List<Key> Keys = new List<Key>();

		public EncryptedStorage(Storage storage, IGenerateID idGenerator) : base(storage)
		{
			if (idGenerator == null)
				this.idGenerator = new NullID();
			else
				this.idGenerator = idGenerator;
		}

		public void AddKey(Key key)
		{
			this.Keys.Add(key);
		}

		public override void WriteBlob(Blob blob)
		{
			if (Keys.Count == 0)
				throw new InvalidOperationException("EncryptedStorage must have at least one key");
			
			//Encrypt
			Encrypt(blob);
			
			//Generate CustomID
			blob.CustomID = idGenerator.GetID(blob);
			
			//Reuse already existsing CustomID
			BlobHash hash = GetBlobHash(blob.CustomID);
			if (hash != null)
			{
				blob.BlobHash = hash;
				return;
			}
			
			foreach (Key k in Keys)
				blob.AddKey(k);
			base.WriteBlob(blob);
		}

		public override Blob ReadBlob(BlobHash id)
		{
			Blob blob = base.ReadBlob(id);
			
			if (blob.Keys != null)
				Decrypt(blob);
			
			return blob;
		}

		void Encrypt(Blob blob)
		{
			if (blob.Keys != null)
				throw new InvalidOperationException("Can only encrypt once");
			
			//Generate key
			blob.Keys = new BlobKeys();
			blob.Keys.RM.GenerateIV();
			blob.Keys.RM.GenerateKey();
			
			//Encrypt data
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, blob.Keys.RM.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(blob.Data, 0, blob.Data.Length);
				}
				blob.Data = ms.ToArray();
			}
			
			//Generate Hash
			blob.BlobHash = new BlobHash(Hash.ComputeHash(blob.Data));
		}

		BlobKey Decrypt(List<BlobKey> pairs)
		{
			foreach (BlobKey pair in pairs)
			{
				foreach (PrivateKey k in this.Keys)
				{
					BlobKey clear = new BlobKey();
					clear.bytes = k.Decrypt(pair.bytes);
					return clear;
				}
			}
			return null;
		}

		void Decrypt(Blob blob)
		{
			if (blob.Keys == null)
				throw new InvalidDataException("Missing keys");
			
			//Decrypt Key
			BlobKey key = Decrypt(blob.Keys.Keys);
			if (key == null)
				throw new InvalidDataException("Could not decrypt key");
			
			blob.Keys.RM.Key = key.bytes;
			
			//Decrypt Data
			blob.Keys.RM.Mode = CipherMode.CBC;
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, blob.Keys.RM.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(blob.Data, 0, blob.Data.Length);
				}
				blob.Data = ms.ToArray();
			}
			blob.ClearHash = Hash.ComputeHash(blob.Data);
		}

	}
}

