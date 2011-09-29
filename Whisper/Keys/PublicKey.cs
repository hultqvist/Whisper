using System;
using System.Security.Cryptography;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Keys
{
	public partial class PublicKey : IKey
	{
		private RSACryptoServiceProvider rsa;

		public PublicKey(byte[] modulus, byte[] exponent)
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = modulus;
			rp.Exponent = exponent;
			rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(rp);
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		protected PublicKey()
		{
		}

		protected void BeforeSerialize()
		{
			RSAParameters rp = rsa.ExportParameters(false);
			this.Modulus = rp.Modulus;
			this.Exponent = rp.Exponent;
		}

		protected void AfterDeserialize()
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = this.Modulus;
			rp.Exponent = this.Exponent;
			rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(rp);
		}

		public byte[] Encrypt(byte[] data)
		{
			return rsa.Encrypt(data, false);
		}

		public bool Verify(byte[] data, byte[] signature)
		{
			return rsa.VerifyData(data, new SHA256Managed(), signature);
		}

		/// <summary>
		/// The SHA1 fingerprint of the RSA public key
		/// </summary>
		public override string ToString()
		{
			RSAParameters rsaParameters = rsa.ExportParameters(false);
			MemoryStream ms = new MemoryStream();
			ms.Write(rsaParameters.Modulus, 0, rsaParameters.Modulus.Length);
			ms.Write(rsaParameters.Exponent, 0, rsaParameters.Exponent.Length);
			Hash hash = Hash.ComputeHash(ms.ToArray());
			return hash.ToString();
		}
	}
}

