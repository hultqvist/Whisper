using System;
using System.IO;
using System.Security.Cryptography;

namespace Whisper.Keys
{
	public partial class PrivateKey : IKey
	{
		RSACryptoServiceProvider rsa;

		/// <summary>
		/// Create a new key pair
		/// </summary>
		public PrivateKey(bool generate)
		{
			rsa = new RSACryptoServiceProvider();
			rsa.ExportParameters(true);
		}

		/// <summary>
		/// For testing only
		/// </summary>
		public PrivateKey(string xmlParameters)
		{
			rsa.FromXmlString(xmlParameters);
		}

		/// <summary>
		/// For serialization only
		/// </summary>
		protected PrivateKey()
		{
		}

		protected void BeforeSerialize()
		{
			RSAParameters rp = rsa.ExportParameters(false);
			this.Modulus = rp.Modulus;
			this.Exponent = rp.Exponent;
			this.P = rp.P;
			this.Q = rp.Q;
		}

		protected void AfterDeserialize()
		{
			RSAParameters rp = new RSAParameters();
			rp.Modulus = this.Modulus;
			rp.Exponent = this.Exponent;
			rp.P = this.P;
			rp.Q = this.Q;
			rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(rp);
		}

		PublicKey _pubkey = null;

		public PublicKey PublicKey {
			get {
				if (_pubkey == null)
					_pubkey = new PublicKey(this.Modulus, this.Exponent);
				return _pubkey;
			}
		}

		public byte[] Decrypt(byte[] data)
		{
			try
			{
				return rsa.Decrypt(data, false);
			}
			catch (CryptographicException)
			{
				return null;
			}
		}

		public byte[] Sign(byte[] data)
		{
			return rsa.SignData(data, new SHA256Managed());
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

