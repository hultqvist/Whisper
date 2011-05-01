using System;
using Whisper;
using Whisper.Blobs;
using System.Security.Cryptography;
using System.IO;
namespace Whisper.ID
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

		public CustomID GetID(Blob blob)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			sender.WriteBlob(bw);
			blob.ClearHash.WriteBlob(bw);
			return new CustomID(Hash.ComputeHash(ms.ToArray()));
		}
		
	}
}

