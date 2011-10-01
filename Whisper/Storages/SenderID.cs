using System;
using System.IO;
using System.Security.Cryptography;
using Whisper;
using Whisper.Chunks;
using Whisper.Keys;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Storages
{
	/// <summary>
	/// Generates the same CustomID given a senderKey and the cleartext hash
	/// </summary>
	public class SenderID : IGenerateID
	{
		PrivateKey sender;
		byte[] keyBuffer;
		
		public SenderID (PrivateKey senderKey)
		{
			this.sender = senderKey;
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

