using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Messaging;
using System.IO;
using Whisper.Storing;

namespace Whisper.Chunks
{
	/// <summary>
	/// Contains all the neccesary data to decrypt a chunk.
	/// </summary>
	public partial class ChunkKeys
	{
		public RijndaelManaged RM = new RijndaelManaged() { KeySize = 256, Mode = CipherMode.CBC};

		public byte[] IV {
			get { return RM.IV; }
			set { RM.IV = value; }
		}

		public List<ChunkKey> Keys = new List<ChunkKey>();

		public void AddKey(ChunkKey keyPair)
		{
			Keys.Add(keyPair);
		}
	}
}

