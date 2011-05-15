using System;
using System.Security.Cryptography;
using System.IO;
using Whisper.Blobing;

namespace Whisper
{
	/// <summary>
	/// User key
	/// </summary>
	public abstract class Key : BinaryBlob
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
		
	}
}

