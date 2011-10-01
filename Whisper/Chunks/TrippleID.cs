using System;
using System.IO;

namespace Whisper.Chunks
{
	/// <summary>
	/// A triplet representing a single blob.
	/// Hash of the possibly encrypted blob,
	/// CustomID and hash of the cleartext data.
	/// </summary>
	public partial class TrippleID
	{
		/// <summary>
		/// Hash of ciphertext Data
		/// </summary>
		public ChunkHash ChunkHash { get; set; }
		/// <summary>
		/// Sender choosen id, see Whisper.Storages.*ID for examples
		/// </summary>
		public CustomID CustomID { get; set; }
		/// <summary>
		/// Hash of cleartext Data
		/// </summary>
		public ClearHash ClearHash { get; set; }

		public TrippleID(Chunk chunk)
		{
			this.ChunkHash = chunk.DataHash;
			this.CustomID = chunk.CustomID;
			this.ClearHash = chunk.ClearHash;
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

