using System;
using System.IO;
using System.Security.Cryptography;
using Whisper.Chunks;
using Whisper.Keys;
using ProtocolBuffers;

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

		public CustomID GetID(Chunk chunk)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PublicKey.Serialize(ms, recipient);
				ProtocolParser.WriteBytes(ms, chunk.ClearHash.bytes);
				return CustomID.FromBytes(Hash.ComputeHash(ms.ToArray()).bytes);
			}
		}
	}
}

