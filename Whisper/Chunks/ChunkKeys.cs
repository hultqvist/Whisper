using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Messaging;
using System.IO;

namespace Whisper.Chunks
{
	/// <summary>
	/// Contains all the neccesary data to decrypt a blob.
	/// </summary>
	public class ChunkKeys : BinaryChunk
	{
		public RijndaelManaged RM = new RijndaelManaged();

		public byte[] IV {
			get { return RM.IV; }
			set { RM.IV = value; }
		}

		public List<ChunkKey> Keys = new List<ChunkKey>();

		public ChunkKeys()
		{
			RM.KeySize = 256;
			RM.Mode = CipherMode.CBC;
		}
		public void AddKey(ChunkKey keyPair)
		{
			Keys.Add(keyPair);
		}

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			writer.Write((int) Keys.Count);
			if (Keys.Count == 0)
				return;
			writer.Write(IV);
			foreach (ChunkKey pair in Keys)
			{
				writer.Write(pair.bytes);
			}
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			if (count == 0)
				return;
			IV = reader.ReadBytes(16);
			for(int n = 0; n < count; n++)
			{
				ChunkKey pair = new ChunkKey();
				pair.bytes = reader.ReadBytes(128);
				Keys.Add(pair);
			}
		}
		
		#endregion
		
	}
}

