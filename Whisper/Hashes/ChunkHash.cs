using System;
using System.IO;

namespace Whisper
{
	/// <summary>
	/// A hash of the chunk data, being cleartext or encrypted data
	/// </summary>
	public class ChunkHash : Hash
	{
		private ChunkHash (byte[] hash) : base(hash)
		{
		}

		public static new ChunkHash FromString (string id)
		{
			return new ChunkHash (Hash.FromString (id).bytes);
		}

		public new static ChunkHash FromHashBytes (byte[] bytes)
		{
			if (bytes == null)
				return null;
			return new ChunkHash (bytes);
		}

		public new static ChunkHash ComputeHash (byte[] data)
		{
			Hash hash = Hash.ComputeHash (data);
			return new ChunkHash (hash.bytes);
		}

		public static byte[] GetBytes (ChunkHash customID)
		{
			if (customID == null)
				return null;
			return customID.bytes;
		}
	}
}
