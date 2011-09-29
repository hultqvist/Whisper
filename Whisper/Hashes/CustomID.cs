using System;
using System.IO;

namespace Whisper
{
	/// <summary>
	/// Sender generated ID for a chunk.
	/// See classes in Whisper.Storing for examples.
	/// </summary>
	public class CustomID
	{
		public readonly byte[] bytes;

		private CustomID(byte[] id)
		{
			this.bytes = id;
		}

		public static CustomID FromString(string id)
		{
			byte[] bytes = HexParser.ParseHex(id);
			return new CustomID(bytes);
		}

		public static CustomID FromBytes(byte[] bytes)
		{
			if (bytes == null)
				return null;
			return new CustomID(bytes);
		}

		/// <summary>
		/// Return null if customID is null, otherwise the id bytes.
		/// </summary>
		public static byte[] GetBytes(CustomID customID)
		{
			if (customID == null)
				return null;
			return customID.bytes;
		}

	}
}

