using System;
using Whisper;
using Whisper.Chunks;
using System.Security.Cryptography;
using System.IO;
namespace Whisper.Storing
{
	/// <summary>
	/// Generates the same CustomID given a senderKey and the cleartext hash
	/// </summary>
	public class SenderID : IGenerateID
	{
		PrivateKey sender;
		public SenderID(PrivateKey senderKey)
		{
			this.sender = senderKey;
		}

		public CustomID GetID(Chunk blob)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			sender.WriteChunk(bw);
			blob.ClearHash.WriteChunk(bw);
			return new CustomID(Hash.ComputeHash(ms.ToArray()));
		}
		
	}
}

