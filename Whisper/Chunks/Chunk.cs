using System;
using System.Collections.Generic;
using Whisper.Messaging;
using System.IO;
using System.Security.Cryptography;
using Whisper.Keys;

namespace Whisper.Chunks
{
	/// <summary>
	/// Basic unit for storing data in transit, on disk or over network.
	/// </summary>
	public class Chunk
	{

		/// <summary>
		/// Sender selected ID for this chunk
		/// </summary>
		public CustomID CustomID;
		/// <summary>
		/// Hash of Data
		/// </summary>
		public ChunkHash DataHash;
		/// <summary>
		/// Chunk data as stored, can be either cleartext or encrypted.
		/// </summary>
		public byte[] Data { get; set; }

		public TrippleID TrippleID;

		/// <summary>
		/// Keys to decrypt Data
		/// </summary>
		public ChunkKeys Keys = null;

		/// <summary>
		/// Hash of cleartext data
		/// </summary>
		public ClearHash ClearHash;

		public Chunk()
		{
		}

		public Chunk(byte[] buffer)
		{
			//Hash data
			this.Data = buffer;

			this.ClearHash = ClearHash.ComputeHash(buffer);
			this.DataHash = ChunkHash.FromHashBytes(this.ClearHash.bytes);
			this.TrippleID = new TrippleID(this);
		}

		public override string ToString()
		{
			return "Chunk(" + Data.Length + " bytes)";
		}

		/// <summary>
		/// Add public keys that will be able to decrypt the chunk.
		/// </summary>
		/// <param name="key">
		/// A <see cref="Key"/>
		/// </param>
		public void AddKey(PublicKey key)
		{
			if (Keys == null)
				throw new InvalidOperationException("Must call Encrypt before");
			
			ChunkKey bk = new ChunkKey();
			bk.bytes = key.Encrypt(Keys.RM.Key);
			Keys.AddKey(bk);
		}

		/// <summary>
		/// Verify the ClearID.ClearHash against chunk data.
		/// The data is assumed to be decrypted, otherwise the verification will fail.
		/// </summary>
		public bool Verify(TrippleID id)
		{
			if (id.ClearHash.Equals(ClearHash) == false)
				return false;
			return true;
		}
	}
}

