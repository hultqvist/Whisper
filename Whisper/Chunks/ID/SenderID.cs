using System;
using System.IO;
using System.Security.Cryptography;
using Whisper;
using Whisper.Chunks;
using Whisper.Encryption;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Chunks.ID
{
	/// <summary>
	/// Generates the same CustomID given a senderKey and the cleartext hash.
	/// Pro: Sender only has to send one copy to repo for multiple recepient.
	/// Con: Repo knows that the recepients are receiving the same data.
	/// Con: Receiver working with repo can tell who else got the same data.
	/// </summary>
	public class SenderID : IGenerateID
	{
		byte[] keyBuffer;
		
		public SenderID (PrivateKey senderKey)
		{
			keyBuffer = Encoding.ASCII.GetBytes (senderKey.ToXml ());
		}

		public CustomID GetID (Chunk chunk)
		{
			using (MemoryStream ms = new MemoryStream()) {
				ms.Write (keyBuffer, 0, keyBuffer.Length);
				ProtocolParser.WriteBytes (ms, chunk.ClearHash.bytes);
				return CustomID.FromBytes (Hash.ComputeHash (ms.ToArray ()).bytes);
			}
		}
		
	}
}

