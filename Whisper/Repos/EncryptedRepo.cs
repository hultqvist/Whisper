using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Chunks;
using Whisper.Repos;
using Whisper.Encryption;

namespace Whisper.Repos
{
	/// <summary>
	/// Encrypt all data before being passed to the backend repo
	/// </summary>
	public class EncryptedRepo : RepoFilter
	{
		/// <summary>
		/// Only used for finding decryption key
		/// </summary>
		private readonly KeyStorage keyStorage;
		/// <summary>
		/// keys used for encryption
		/// </summary>
		public List<PublicKey> recipientKeys = new List<PublicKey> ();

		/// <summary>
		/// Encrypts all chunks before sending them to the backend repo
		/// </summary>
		/// <param name="backendRepo">
		/// This is where the encrypted chunks are sent
		/// </param>
		/// <param name="keyStorage">
		/// If decrypting, this is where we look for private keys
		/// </param>
		public EncryptedRepo (Repo backendRepo, KeyStorage keyStorage) : base(backendRepo)
		{
			this.keyStorage = keyStorage;
		}

		public override string ToString ()
		{
			return "Encrypted(" + base.ToString () + ")";
		}

		/// <summary>
		/// Add recipient keys
		/// </summary>
		public void AddKey (PublicKey key)
		{
			this.recipientKeys.Add (key);
		}

		public override ChunkHash WriteChunk (Chunk chunk)
		{
			if (recipientKeys.Count == 0)
				throw new InvalidOperationException ("EncryptedRepo must have at least one key");
			
			//Encrypt
			Chunk encryptedChunk = Encrypt (chunk);

			//Reuse already existsing CustomID
			if (chunk.CustomID != null) {
				ChunkHash hash = GetCustomHash (chunk.CustomID);
				if (hash != null)
					return hash;
			}

			return base.WriteChunk (encryptedChunk);
		}

		public override Chunk ReadChunk (ChunkHash id)
		{
			Chunk chunk = base.ReadChunk (id);

			try {
				return Decrypt (chunk);
			} catch (Exception) {
				return null;
			}
		}

		/// <summary>
		/// Encrypt chunk data into a new chunk
		/// </summary>
		Chunk Encrypt (Chunk chunk)
		{
			KeysHeader kh = new KeysHeader ();
			//Generate key
			kh.RM.GenerateIV ();
			kh.RM.GenerateKey ();

			//Add recipient keys
			foreach (PublicKey pubkey in recipientKeys) {
				byte[] bk = pubkey.Encrypt (kh.RM.Key);
				kh.EncryptedKeys.Add (bk);
			}

			//Encrypt data
			using (MemoryStream ms = new MemoryStream()) {
				//Headers
				ProtocolBuffers.ProtocolParser.WriteBytes (ms, KeysHeader.SerializeToBytes (kh));
				//Encrypted data
				using (CryptoStream cs = new CryptoStream(ms, kh.RM.CreateEncryptor(), CryptoStreamMode.Write)) {
					cs.Write (chunk.Data, 0, chunk.Data.Length);
				}
				return new Chunk (ms.ToArray ());
			}
		}

		/// <summary>
		/// Tries to decrypt all encrypted keys using all available private keys
		/// </summary>
		byte[] Decrypt (List<byte[]> encrypted_keys)
		{
			foreach (byte[] encrypted_key in encrypted_keys) {
				foreach (PrivateKey privateKey in this.keyStorage.PrivateKeys) {
					byte[] key = privateKey.Decrypt (encrypted_key);
					if (key == null)
						continue;

					return key;
				}
			}
			return null;
		}

		/// <summary>
		/// Return new chunk with decrypted data
		/// </summary>
		Chunk Decrypt (Chunk chunk)
		{
			//Read header
			int headerSize;
			KeysHeader head;
			using (MemoryStream chunkStream = new MemoryStream(chunk.Data)) {
				head = KeysHeader.Deserialize (ProtocolBuffers.ProtocolParser.ReadBytes (chunkStream));
				headerSize = (int)chunkStream.Position;
			}

			//Decrypt Key
			byte[] key = Decrypt (head.EncryptedKeys);
			if (key == null)
				return null;

			head.RM.Key = key;

			//Decrypt Data
			head.RM.Mode = CipherMode.CBC;
			using (MemoryStream ms = new MemoryStream()) {
				using (CryptoStream cs = new CryptoStream(ms, head.RM.CreateDecryptor(), CryptoStreamMode.Write)) {
					cs.Write (chunk.Data, headerSize, chunk.Data.Length - headerSize);
				}
				return new Chunk (ms.ToArray ());
			}
		}
	}
}

