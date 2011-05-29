using System;
using Whisper.Chunks;
using System.Security.Cryptography;
using System.IO;
namespace Whisper.Storing
{
	/// <summary>
	/// Always generates the same CustomID for one specific recipient and cleartext data.
	/// This will optimize traffic and storage when multiple users are sending the same data to a recipient.
	/// </summary>
	public class RecipientID : IGenerateID
	{
		PublicKey recipient;
		public RecipientID(PublicKey recipient)
		{
			this.recipient = recipient;
		}

		public CustomID GetID(Chunk blob)
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			recipient.WriteChunk(bw);
			blob.ClearHash.WriteChunk(bw);
			return new CustomID(Hash.ComputeHash(ms.ToArray()));
		}
	}
}

