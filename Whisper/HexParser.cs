using System;
namespace Whisper
{
	public static class HexParser
	{
		public static byte[] ParseHex(string hex)
		{
			int offset = hex.StartsWith("0x") ? 2 : 0;
			if ((hex.Length % 2) != 0)
			{
				throw new ArgumentException("Invalid length: " + hex.Length);
			}
			byte[] ret = new byte[(hex.Length - offset) / 2];
			
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = (byte) ((ParseNybble(hex[offset]) << 4) | ParseNybble(hex[offset + 1]));
				offset += 2;
			}
			return ret;
		}

		static int ParseNybble(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - '0';
			}
			if (c >= 'A' && c <= 'F')
			{
				return c - 'A' + 10;
			}
			if (c >= 'a' && c <= 'f')
			{
				return c - 'a' + 10;
			}
			throw new ArgumentException("Invalid hex digit: " + c);
		}
	}
}

