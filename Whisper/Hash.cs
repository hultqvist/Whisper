using System;
using System.Security.Cryptography;
using System.IO;
using Whisper.Blobing;
namespace Whisper
{

	public class Hash : BinaryBlob
	{
		public readonly byte[] bytes;

		#region Constructors and Hash creators

		public Hash(byte[] hash)
		{
			if (hash.Length != 32)
				throw new ArgumentException("hash must be 32 bytes", "hash");
			
			this.bytes = hash;
		}

		public Hash(Hash hash)
		{
			this.bytes = hash.bytes;
		}

		public static Hash FromString(string id)
		{
			byte[] bytes = HexParser.ParseHex(id);
			return new Hash(bytes);
		}

		public static Hash ComputeHash(byte[] data)
		{
			SHA256Managed sha = new SHA256Managed();
			return new Hash(sha.ComputeHash(data));
		}

		#endregion

		public override string ToString()
		{
			return BitConverter.ToString(bytes).Replace("-", "");
		}

		#region Equals

		public override bool Equals(object obj)
		{
			Hash h2 = obj as Hash;
			if (h2 == null)
				return false;
			
			if (this == h2)
				return true;
			if (h2 == null)
				return false;
			if (bytes.Length != h2.bytes.Length)
				return false;
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] != h2.bytes[i])
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return bytes.GetHashCode();
		}

		#endregion

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			writer.Write(bytes);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			throw new NotImplementedException("Use Hash.FromBlob instead");
		}

		internal static Hash FromBlob(BinaryReader reader)
		{
			return new Hash(reader.ReadBytes(32));
		}
		#endregion
		
	}
}

