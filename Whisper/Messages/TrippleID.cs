using System;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messages
{
	/// <summary>
	/// A triplet representing a single chunk.
	/// Hash of the possibly encrypted chunk,
	/// CustomID and hash of the cleartext data.
	/// </summary>
	public partial class TrippleID
	{
		/// <summary>
		/// Hash of ciphertext Data
		/// </summary>
		public ChunkHash ChunkHash { get; set; }
		/// <summary>
		/// Sender choosen id, see Whisper.Repos.*ID for examples
		/// </summary>
		public CustomID CustomID { get; set; }
		/// <summary>
		/// Hash of cleartext Data
		/// </summary>
		public ClearHash ClearHash { get; set; }

		public TrippleID(ClearHash clear, ChunkHash chunk, CustomID custom)
		{
			this.ClearHash = clear;
			this.ChunkHash = chunk;
			this.CustomID = custom;
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		private TrippleID()
		{
		}

		protected void BeforeSerialize()
		{
			this.ChunkHashBytes = ChunkHash.GetBytes(this.ChunkHash);
			this.ClearHashBytes = ClearHash.GetBytes(this.ClearHash);
			this.CustomIdBytes = CustomID.GetBytes(this.CustomID);
		}

		protected void AfterDeserialize()
		{
			this.ChunkHash = ChunkHash.FromHashBytes(this.ChunkHashBytes);
			this.ClearHash = ClearHash.FromHashBytes(this.ClearHashBytes);
			this.CustomID = CustomID.FromBytes(this.CustomIdBytes);
		}

		public override string ToString()
		{
			return "Clear " + ChunkHash + " [" + ClearHash + "]";
		}
	}
}

