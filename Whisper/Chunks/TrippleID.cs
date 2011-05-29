using System;
using System.IO;
namespace Whisper.Chunks
{
	/// <summary>
	/// A triplet representing a single blob.
	/// Hash of the possibly encrypted blob,
	/// CustomID and hash of the cleartext data.
	/// </summary>
	public class TrippleID : BinaryChunk
	{
		/// <summary>
		/// Hash of ciphertext Data
		/// </summary>
		public ChunkHash ChunkHash { get; set; }
		/// <summary>
		/// Sender choosen id, see Whisper.Storing.* for examples
		/// </summary>
		public CustomID CustomID { get; set; }
		/// <summary>
		/// Hash of cleartext Data
		/// </summary>
		public Hash ClearHash { get; set; }

		private TrippleID()
		{
		}

		public TrippleID(Chunk blob)
		{
			this.ChunkHash = blob.ChunkHash;
			this.CustomID = blob.CustomID;
			this.ClearHash = blob.ClearHash;
		}

		public override string ToString()
		{
			return "Clear " + ChunkHash + " [" + ClearHash + "]";
		}

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			this.ChunkHash.WriteChunk(writer);
			this.CustomID.WriteChunk(writer);
			this.ClearHash.WriteChunk(writer);
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			this.ChunkHash = ChunkHash.FromChunk(reader);
			this.CustomID = CustomID.FromChunk(reader);
			this.ClearHash = Hash.FromChunk(reader);
		}

		internal static TrippleID FromBlob(BinaryReader reader)
		{
			TrippleID id = new TrippleID();
			id.ReadChunk(reader);
			return id;
		}

		#endregion

	}
}

