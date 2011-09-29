using System;
using System.IO;
using System.Security.Cryptography;
using Whisper;
using Whisper.Chunks;
using Whisper.Keys;
using ProtocolBuffers;

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

		public CustomID GetID(Chunk chunk)
		{
			MemoryStream ms = new MemoryStream();
			Serializer.Write(ms, sender);
			ProtocolParser.WriteBytes(ms, chunk.ClearHash.bytes);
			return CustomID.FromBytes(Hash.ComputeHash(ms.ToArray()).bytes);
		}
		
	}
}

