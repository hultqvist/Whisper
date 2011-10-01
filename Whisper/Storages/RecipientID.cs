using System;
using System.IO;
using System.Security.Cryptography;
using Whisper.Chunks;
using Whisper.Keys;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Storages
{
	/// <summary>
	/// Always generates the same CustomID for one specific recipient and cleartext data.
	/// This will optimize traffic and storage when multiple users are sending the same data to a recipient.
	/// </summary>
	public class RecipientID : IGenerateID
	{
		PublicKey recipient;
		byte[] keyBuffer;
		
		public RecipientID(PublicKey recipient)
		{
			this.recipient = recipient;
			keyBuffer = Encoding.ASCII.GetBytes(recipient.ToXml());
		}

		public CustomID GetID(Chunk chunk)
		{
			
			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(keyBuffer, 0, keyBuffer.Length);
				ProtocolParser.WriteBytes(ms, chunk.ClearHash.bytes);
				return CustomID.FromBytes(Hash.ComputeHash(ms.ToArray()).bytes);
			}
		}
	}
}

