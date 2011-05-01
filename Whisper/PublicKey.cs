using System;
using System.Security.Cryptography;
using System.IO;
namespace Whisper
{
	public class PublicKey : Key
	{
		public PublicKey()
		{
		}

		public PublicKey(string xmlKey)
		{
			rsa.FromXmlString(xmlKey);
		}

		internal override void WriteBlob(BinaryWriter writer)
		{
			RSAParameters rp = rsa.ExportParameters(false);
			WriteVarBytes(writer, rp.Modulus);
			WriteVarBytes(writer, rp.Exponent);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = ReadVarBytes(reader);
			rp.Exponent = ReadVarBytes(reader);
		}
	}
}

