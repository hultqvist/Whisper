using System;
using System.IO;
namespace Whisper
{
	/// <summary>
	/// A hash of the blob data in transit
	/// </summary>
	public class BlobHash : Hash
	{
		public BlobHash(Hash hash) : base(hash)
		{
		}

		public static new BlobHash FromString(string id)
		{
			return new BlobHash(Hash.FromString(id));
		}

		internal static new BlobHash FromBlob(BinaryReader reader)
		{
			return new BlobHash(Hash.FromBlob(reader));
		}

	}
}
