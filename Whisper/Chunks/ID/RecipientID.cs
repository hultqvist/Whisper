using System;
using System.IO;
using System.Security.Cryptography;
using Whisper.Chunks;
using Whisper.Encryption;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Chunks.ID
{
	/// <summary>
	/// Always generates the same CustomID for one specific recipient and cleartext data.
	/// Pro: Sender does not have to send data if already sent by another sender.
	/// Con: Repo knows that senders are sending the same data.
	/// Con: Repo working with sender can know what other data other senders are sending to receiver.
	/// </summary>
	public class RecipientID : IGenerateID
	{
		byte[] keyBuffer;
		
		public RecipientID(PublicKey recipient)
		{
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

