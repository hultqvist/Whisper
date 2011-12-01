using System;
using System.IO;
using System.Security.Cryptography;
using Whisper;
using Whisper.Chunks;
using Whisper.Keys;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Repos
{
	/// <summary>
	/// Generates the same CustomID given a senderKey and the cleartext hash.
	/// Pro: Sender only has to send one copy to storage for multiple recepient.
	/// Con: Storage knows that the recepients are receiving the same data.
	/// Con: Receiver working with storage can tell who else got the same data.
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

