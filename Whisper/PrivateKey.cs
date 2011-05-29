using System;
using System.IO;
using System.Security.Cryptography;
namespace Whisper
{
	public class PrivateKey : PublicKey
	{
		public PrivateKey()
		{
		
		}

		public PrivateKey(string xmlKey)
		{
			rsa.FromXmlString(xmlKey);
		}

		public PublicKey PublicKey {
			get { return new PublicKey(rsa.ToXmlString(false)); }
		}

		public byte[] Decrypt(byte[] data)
		{
			return rsa.Decrypt(data, false);
		}

		public byte[] Sign(byte[] data)
		{
			return rsa.SignData(data, new SHA256Managed());
		}

		internal override void WriteChunk(BinaryWriter writer)
		{
			RSAParameters rp = rsa.ExportParameters(true);
			WriteVarBytes(writer, rp.Modulus);
			WriteVarBytes(writer, rp.Exponent);
			WriteVarBytes(writer, rp.P);
			WriteVarBytes(writer, rp.Q);
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = ReadVarBytes(reader);
			rp.Exponent = ReadVarBytes(reader);
			rp.P = ReadVarBytes(reader);
			rp.Q = ReadVarBytes(reader);
		}
		
	}
}

