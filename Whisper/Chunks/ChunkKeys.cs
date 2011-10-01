using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Messages;
using System.IO;
using Whisper.Storages;

namespace Whisper.Chunks
{
	/// <summary>
	/// Contains all the neccesary data to decrypt a chunk.
	/// </summary>
	public partial class ChunkKeys
	{
		public RijndaelManaged RM = new RijndaelManaged () { KeySize = 256, Mode = CipherMode.CBC};

		public byte[] IV {
			get { return RM.IV; }
			set { RM.IV = value; }
		}
		
		public ChunkKeys ()
		{
			this.EncryptedKeys = new List<byte[]> ();
		}
	}
}

