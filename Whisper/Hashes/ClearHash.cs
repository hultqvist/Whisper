using System;

namespace Whisper
{
	public class ClearHash : Hash
	{
		private ClearHash (byte[] hash) : base(hash)
		{
		}

		public static new ClearHash FromString (string id)
		{
			return new ClearHash (Hash.FromString (id).bytes);
		}

		public new static ClearHash FromHashBytes (byte[] bytes)
		{
			if (bytes == null)
				return null;
			return new ClearHash (bytes);
		}

		public new static ClearHash ComputeHash (byte[] data)
		{
			Hash hash = Hash.ComputeHash (data);
			return new ClearHash (hash.bytes);
		}

		public static byte[] GetBytes (ClearHash customID)
		{
			if (customID == null)
				return null;
			return customID.bytes;
		}
	}
}

