using System;
using System.Collections.Generic;
using Whisper.Messages;
using System.IO;
using System.Security.Cryptography;
using Whisper.Encryption;

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
		public readonly ChunkHash ChunkHash;
		/// <summary>
		/// Chunk data as stored, can be either cleartext or encrypted.
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// Hash of cleartext data
		/// </summary>
		public ClearHash ClearHash;

		public Chunk ()
		{
		}

		public Chunk (byte[] buffer)
		{
			//Hash data
			this.Data = buffer;

			this.ClearHash = ClearHash.ComputeHash (buffer);
			this.ChunkHash = ChunkHash.FromHashBytes (this.ClearHash.bytes);
		}

		public override string ToString ()
		{
			return "Chunk(" + Data.Length + " bytes)";
		}

		/// <summary>
		/// Verify the ClearID.ClearHash against chunk data.
		/// The data is assumed to be decrypted, otherwise the verification will fail.
		/// </summary>
		public bool Verify (ChunkHash ch)
		{
			if (ch.Equals (ChunkHash) == false)
				return false;
			return true;
		}
	}
}

