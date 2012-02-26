using System;
using System.Security.Cryptography;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Encryption
{
	public partial class PublicKey : IKey
	{
		public string Name { get; set; }
		
		private RSACryptoServiceProvider rsa;

		public PublicKey (string xml)
		{
			rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (xml);
		}
		
		public string ToXml ()
		{
			return rsa.ToXmlString (false);
		}

		public byte[] Encrypt (byte[] data)
		{
			return rsa.Encrypt (data, false);
		}

		public bool Verify (byte[] data, byte[] signature)
		{
			return rsa.VerifyData (data, new SHA256Managed (), signature);
		}

		/// <summary>
		/// The SHA1 fingerprint of the RSA public key
		/// </summary>
		public override string ToString ()
		{
			RSAParameters rsaParameters = rsa.ExportParameters (false);
			MemoryStream ms = new MemoryStream ();
			ms.Write (rsaParameters.Modulus, 0, rsaParameters.Modulus.Length);
			ms.Write (rsaParameters.Exponent, 0, rsaParameters.Exponent.Length);
			Hash hash = Hash.ComputeHash (ms.ToArray ());
			return "(public) " + Name + " " + hash.ToString ();
		}
	}
}

