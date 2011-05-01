using System;
using System.IO;
namespace Whisper.Blobs
{
	/// <summary>
	/// A triplet representing the a single blob including its cleartext hash
	/// </summary>
	public class ClearID : BinaryBlob
	{
		/// <summary>
		/// Hash of ciphertext Data
		/// </summary>
		public BlobHash BlobHash { get; set; }
		/// <summary>
		/// Sender choosen id, see Whisper.ID.* for examples
		/// </summary>
		public CustomID CustomID { get; set; }
		/// <summary>
		/// Hash of cleartext Data
		/// </summary>
		public Hash ClearHash { get; set; }

		private ClearID()
		{
		}

		public ClearID(Blob blob)
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

		internal static ClearID FromBlob(BinaryReader reader)
		{
			ClearID id = new ClearID();
			id.ReadBlob(reader);
			return id;
		}

		#endregion

	}
}

