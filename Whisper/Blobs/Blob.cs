using System;
using System.Collections.Generic;
using Whisper.Messages;
using System.IO;
using System.Security.Cryptography;

namespace Whisper.Blobs
{
	/// <summary>
	/// Basic unit for storing data in transit, on disk or over network.
	/// </summary>
	public class Blob
	{
		/// <summary>
		/// Sender selected ID for this blob
		/// </summary>
		public CustomID CustomID;
		/// <summary>
		/// Hash of Data
		/// </summary>
		public BlobHash BlobHash;
		/// <summary>
		/// Blob data as stored, can be either cleartext or encrypted.
		/// </summary>
		public byte[] Data;

		private ClearID clearID;
		public ClearID ClearID {
			get {
				if (clearID == null)
					clearID = new ClearID(this);
				return clearID;
			}
		}

		/// <summary>
		/// Keys to decrypt Data
		/// </summary>
		public BlobKeys Keys = null;

		/// <summary>
		/// Hash of cleartext data
		/// </summary>
		public Hash ClearHash;

		public Blob()
		{
		}

		public Blob(byte[] buffer)
		{
			//Hash data
			this.Data = buffer;

			this.ClearHash = Hash.ComputeHash(buffer);
			this.BlobHash = new BlobHash(ClearHash);
		}

		/// <summary>
		/// Add public keys that will be able to decrypt the blob.
		/// </summary>
		/// <param name="key">
		/// A <see cref="Key"/>
		/// </param>
		public void AddKey(Key key)
		{
			if (Keys == null)
				throw new InvalidOperationException("Must call Encrypt before");
			
			BlobKey bk = new BlobKey();
			bk.bytes = key.Encrypt(Keys.RM.Key);
			Keys.AddKey(bk);
		}

		/// <summary>
		/// Verify the ClearID.ClearHash against blob data.
		/// The data is assumed to be decrypted, otherwise the verification will fail.
		/// </summary>
		public bool Verify(ClearID id)
		{
			if (id.ClearHash.Equals(ClearHash) == false)
				return false;
			return true;
		}
	}
}

