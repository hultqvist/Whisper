using System;
using System.Security.Cryptography;
using System.IO;
using Whisper.Chunks;
namespace Whisper
{
	public class PublicKey : BinaryChunk
	{
		protected readonly RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

		public byte[] Encrypt(byte[] data)
		{
			return rsa.Encrypt(data, false);
		}

		public bool Verify(byte[] data, byte[] signature)
		{
			return rsa.VerifyData(data, new SHA256Managed(), signature);
		}

		public PublicKey()
		{
		}

		public PublicKey(string xmlKey)
		{
			rsa.FromXmlString(xmlKey);
		}

		internal override void WriteChunk(BinaryWriter writer)
		{
			RSAParameters rp = rsa.ExportParameters(false);
			WriteVarBytes(writer, rp.Modulus);
			WriteVarBytes(writer, rp.Exponent);
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = ReadVarBytes(reader);
			rp.Exponent = ReadVarBytes(reader);
		}
	}
}

