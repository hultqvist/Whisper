using System;
using System.IO;
namespace Whisper.Blobing
{
	/// <summary>
	/// A triplet representing a single blob.
	/// Hash of the possibly encrypted blob,
	/// CustomID and hash of the cleartext data.
	/// </summary>
	public class TrippleID : BinaryBlob
	{
		/// <summary>
		/// Hash of ciphertext Data
		/// </summary>
		public BlobHash BlobHash { get; set; }
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

		public TrippleID(Blob blob)
		{
			this.BlobHash = blob.BlobHash;
			this.CustomID = blob.CustomID;
			this.ClearHash = blob.ClearHash;
		}

		public override string ToString()
		{
			return "Clear " + BlobHash + " [" + ClearHash + "]";
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			this.BlobHash.WriteBlob(writer);
			this.CustomID.WriteBlob(writer);
			this.ClearHash.WriteBlob(writer);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			this.BlobHash = BlobHash.FromBlob(reader);
			this.CustomID = CustomID.FromBlob(reader);
			this.ClearHash = Hash.FromBlob(reader);
		}

		internal static TrippleID FromBlob(BinaryReader reader)
		{
			TrippleID id = new TrippleID();
			id.ReadBlob(reader);
			return id;
		}

		#endregion

	}
}

