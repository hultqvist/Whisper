using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Chunks;
using Whisper.Repos;
using Whisper.Keys;

namespace Whisper.Repos
{
	/// <summary>
	/// Encrypt all data before being passed to the backend repo
	/// </summary>
	public class EncryptedRepo : Repo
	{
		private readonly Repo backend;
		private readonly IGenerateID idGenerator;
		private readonly KeyStorage keyStorage;
		/// <summary>
		/// keys used for encryption
		/// </summary>
		public List<PublicKey> recipientKeys = new List<PublicKey>();

		/// <summary>
		/// Encrypts all chunks before sending them to the underlying storage
		/// </summary>
		/// <param name="backendRepo">
		/// This is where the encrypted chunks are sent
		/// </param>
		/// <param name="keyStorage">
		/// If decrypting, this is where we look for private keys
		/// </param>
		public EncryptedRepo(Repo backendRepo, KeyStorage keyStorage) : this(backendRepo, keyStorage, null)
		{
		}

		/// <summary>
		/// Encrypts all chunks before sending them to the underlying storage
		/// </summary>
		/// <param name="backendRepo">
		/// This is where the encrypted chunks are sent
		/// </param>
		/// <param name="keyStorage">
		/// If decrypting, this is where we look for private keys
		/// </param>
		/// <param name="idgenerator">
		/// Used to generate CustomID for all chunks
		/// </param>
		public EncryptedRepo(Repo backendRepo, KeyStorage keyStorage, IGenerateID idGenerator)
		{
			this.backend = backendRepo;
			if (idGenerator == null)
				this.idGenerator = new NullID();
			else
				this.idGenerator = idGenerator;
			this.keyStorage = keyStorage;
		}

		public override string ToString()
		{
			return "Encrypted(" + base.ToString() + ")";
		}

		#region Unmodified requests

		public override List<ChunkHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			backend.StoreMessage(chunkHash);
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			return backend.GetCustomHash(customID);
		}
		#endregion

		/// <summary>
		/// Add recipient keys
		/// </summary>
		public void AddKey(PublicKey key)
		{
			this.recipientKeys.Add(key);
		}

		public override void WriteChunk(Chunk chunk)
		{
			if (recipientKeys.Count == 0)
				throw new InvalidOperationException("EncryptedStorage must have at least one key");
			
			//Encrypt
			Encrypt(chunk);
			
			//Generate CustomID
			chunk.CustomID = idGenerator.GetID(chunk);
			
			//Reuse already existsing CustomID
			if (chunk.CustomID != null)
			{
				ChunkHash hash = GetCustomHash(chunk.CustomID);
				if (hash != null)
				{
					chunk.ChunkHash = hash;
					return;
				}
			}
			
			backend.WriteChunk(chunk);
		}

		public override Chunk ReadChunk(ChunkHash id)
		{
			Chunk chunk = backend.ReadChunk(id);
			
			if (chunk.Keys != null)
				Decrypt(chunk);
			
			return chunk;
		}

		void Encrypt(Chunk chunk)
		{
			if (chunk.Keys != null)
				throw new InvalidOperationException("Can only encrypt once");
			
			//Generate key
			chunk.Keys = new ChunkKeys();
			chunk.Keys.RM.GenerateIV();
			chunk.Keys.RM.GenerateKey();

			//Add recipient keys
			foreach (PublicKey pubkey in recipientKeys)
			{
				byte[] bk = pubkey.Encrypt(chunk.Keys.RM.Key);
				chunk.Keys.EncryptedKeys.Add(bk);
			}
			
			//Encrypt data
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, chunk.Keys.RM.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(chunk.Data, 0, chunk.Data.Length);
				}
				chunk.Data = ms.ToArray();
			}
			
			//Generate Hash
			chunk.ChunkHash = ChunkHash.ComputeHash(chunk.Data);
		}
		
		/// <summary>
		/// Tries to decrypt all encrypted keys using all available private keys
		/// </summary>
		byte[] Decrypt(List<byte[]> encrypted_keys)
		{
			foreach (byte[] encrypted_key in encrypted_keys)
			{
				foreach (PrivateKey privateKey in this.keyStorage.PrivateKeys)
				{
					byte[] key = privateKey.Decrypt(encrypted_key);
					if (key == null)
						continue;

					return key;
				}
			}
			return null;
		}

		bool Decrypt(Chunk chunk)
		{
			if (chunk.Keys == null)
				throw new InvalidDataException("Missing keys");
			
			//Decrypt Key
			byte[] key = Decrypt(chunk.Keys.EncryptedKeys);
			if (key == null)
				return false;
			
			chunk.Keys.RM.Key = key;
			
			//Decrypt Data
			chunk.Keys.RM.Mode = CipherMode.CBC;
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, chunk.Keys.RM.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(chunk.Data, 0, chunk.Data.Length);
				}
				chunk.Data = ms.ToArray();
			}
			chunk.ClearHash = ClearHash.FromHashBytes(Hash.ComputeHash(chunk.Data).bytes);
			return true;
		}

	}
}

