using System;
using System.IO;
namespace Whisper.Blobs
{
	/// <summary>
	/// Sender generated ID for a blob.
	/// See classes in Whisper.ID for examples.
	/// </summary>
	public class CustomID : Hash
	{
		public CustomID(Hash hash) : base(hash)
		{
		}

		public static new CustomID FromString(string id)
		{
			return new CustomID(Hash.FromString(id));
		}

		internal static new CustomID FromBlob(BinaryReader reader)
		{
			return new CustomID(Hash.FromBlob(reader));
		}

	}
}

