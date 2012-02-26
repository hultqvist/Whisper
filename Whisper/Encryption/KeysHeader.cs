using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Messages;
using System.IO;
using Whisper.Repos;

namespace Whisper.Encryption
{
	/// <summary>
	/// The header data of an encrypted chunk
	/// </summary>
	public partial class KeysHeader
	{
		public RijndaelManaged RM = new RijndaelManaged () { KeySize = 256, Mode = CipherMode.CBC};

		public byte[] IV {
			get { return RM.IV; }
			set { RM.IV = value; }
		}

		public List<byte[]> EncryptedKeys = new List<byte[]> ();
	}
}

