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
		private readonly KeyStorage keyStorage;
		/// <summary>
		/// keys used for encryption
		/// </summary>
		public List<PublicKey> recipientKeys = new List<PublicKey>();

		/// <summary>
		/// Encrypts all blobs before sending them to the underlying storage
		/// </summary>
		/// <param name="storage">
		/// This is where the encrypted blobs are sent
		/// </param>
		/// <param name="keyStorage">
		/// If decrypting, this is where we look for private keys
		/// </param>
		public EncryptedStorage(Storage storage, KeyStorage keyStorage) : base(storage)
		{
			this.idGenerator = new NullID();
			this.keyStorage = keyStorage;
		}

		/// <summary>
		/// Encrypts all blobs before sending them to the underlying storage
		/// </summary>
		/// <param name="storage">
		/// This is where the encrypted blobs are sent
		/// </param>
		/// <param name="keyStorage">
		/// If decrypting, this is where we look for private keys
		/// </param>
		/// <param name="idgenerator">
		/// Used to generate CustomID for all blobs
		/// </param>
		public EncryptedStorage(Storage storage, KeyStorage keyStorage, IGenerateID idGenerator) : base(storage)
		{
			if (idGenerator == null)
				throw new ArgumentException("idGenerator cannot be null, use other constructor instead");
			this.idGenerator = idGenerator;
			this.keyStorage = keyStorage;
		}

		/// <summary>
		/// Add recipient keys
		/// </summary>
		public void AddKey(PublicKey key)
		{
			this.recipientKeys.Add(key);
		}

		public override void WriteBlob(Blob blob)
		{
			if (recipientKeys.Count == 0)
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
			
			foreach (PublicKey k in recipientKeys)
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
				foreach (PrivateKey k in this.keyStorage.PrivateKeys)
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

