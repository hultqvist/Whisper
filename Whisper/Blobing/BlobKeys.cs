using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Whisper.Messaging;
using System.IO;

namespace Whisper.Blobing
{
	/// <summary>
	/// Contains all the neccesary data to decrypt a blob.
	/// </summary>
	public class BlobKeys : BinaryBlob
	{
		public RijndaelManaged RM = new RijndaelManaged();

		public byte[] IV {
			get { return RM.IV; }
			set { RM.IV = value; }
		}

		public List<BlobKey> Keys = new List<BlobKey>();

		public BlobKeys()
		{
			RM.KeySize = 256;
			RM.Mode = CipherMode.CBC;
		}
		public void AddKey(BlobKey keyPair)
		{
			Keys.Add(keyPair);
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			writer.Write(IV);
			foreach (BlobKey pair in Keys)
			{
				writer.Write(pair.bytes);
			}
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			IV = reader.ReadBytes(16);
			while (reader.BaseStream.Length - reader.BaseStream.Position >= 128)
			{
				BlobKey pair = new BlobKey();
				pair.bytes = reader.ReadBytes(128);
				Keys.Add(pair);
			}
		}
		
		#endregion
		
	}
}

