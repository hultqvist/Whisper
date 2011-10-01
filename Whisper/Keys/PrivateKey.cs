using System;
using System.IO;
using System.Security.Cryptography;

namespace Whisper.Keys
{
	public partial class PrivateKey : IKey
	{
		public string Name { get; set; }

		RSACryptoServiceProvider rsa;
		
		public static PrivateKey Generate ()
		{
			return new Whisper.Keys.PrivateKey ();
		}
		
		/// <summary>
		/// Generates a new key
		/// </summary>
		private PrivateKey ()
		{
			rsa = new RSACryptoServiceProvider ();
			rsa.ExportParameters (true);
		}
		
		public PrivateKey (string xml)
		{
			rsa = new RSACryptoServiceProvider ();
			rsa.FromXmlString (xml);
		}
		
		public string ToXml ()
		{
			return rsa.ToXmlString (true);
		}
		
		PublicKey _pubkey = null;

		public PublicKey PublicKey {
			get {
				if (_pubkey == null)
					_pubkey = new PublicKey (rsa.ToXmlString (false));
				return _pubkey;
			}
		}

		public byte[] Decrypt (byte[] data)
		{
			try {
				return rsa.Decrypt (data, false);
			} catch (CryptographicException ce) {
				return null;
			}
		}

		public byte[] Sign (byte[] data)
		{
			return rsa.SignData (data, new SHA256Managed ());
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
			return "(private) " + Name + " " + hash.ToString ();
		}
	}
}

