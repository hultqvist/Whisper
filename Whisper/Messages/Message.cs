using System;
using System.IO;
using Whisper.Chunks;
using System.Text;
using System.Collections.Generic;
using Whisper.Keys;
using ProtocolBuffers;

namespace Whisper.Messages
{
	/// <summary>
	/// Unsigned messages
	/// </summary>
	public abstract class Message
	{
		public PublicKey Signature { get; set; }
		
		#region Static Chunk Readers and Writers

		public static Chunk ToChunk (Message m)
		{
			return ToChunk (m, null);
		}

		public static Chunk ToChunk (Message m, PrivateKey signatureKey)
		{
			using (MemoryStream ms = new MemoryStream()) {
				ms.Write (Encoding.ASCII.GetBytes ("Whisper"), 0, 7);
				
				//Message Header
				MessageHeader header = new MessageHeader ();
				if (m is TreeMessage)
					header.MessageId = 1;
				if (m is RouteMessage)
					header.MessageId = 2;
				if (m is ListMessage)
					header.MessageId = 3;

				byte[] messageData = SerializeMessage (m);

				if (signatureKey != null)
					header.Signature = signatureKey.Sign (messageData);
				
				ProtocolParser.WriteBytes (ms, MessageHeader.SerializeToBytes (header));
				ProtocolParser.WriteBytes (ms, messageData);

				//Generate Chunk Data
				byte[] data = ms.ToArray ();
				return new Chunk (data);
			}
		}

		public static byte[] SerializeMessage (Message m)
		{
			if (m is TreeMessage)
				return TreeMessage.SerializeToBytes ((TreeMessage)m);
			if (m is RouteMessage)
				return RouteMessage.SerializeToBytes ((RouteMessage)m);
			if (m is ListMessage)
				return ListMessage.SerializeToBytes ((ListMessage)m);
			throw new NotImplementedException ();
		}

		public static Message FromChunk (Chunk chunk)
		{
			return FromChunk (chunk, null);
		}

		public static Message FromChunk (Chunk chunk, KeyStorage keyStorage)
		{
			if (chunk == null)
				return null;

			using (MemoryStream ms = new MemoryStream(chunk.Data)) {
				byte[] whisper = new byte[7];
				if (ms.Read (whisper, 0, whisper.Length) != whisper.Length)
					throw new InvalidDataException ("Header not right length");
				if (Encoding.ASCII.GetString (whisper) != "Whisper")
					throw new InvalidDataException ("Missing header");
				
				MessageHeader header = MessageHeader.Deserialize (ProtocolParser.ReadBytes (ms));

				byte[] messageBytes = ProtocolParser.ReadBytes (ms);
				Message message;
				switch (header.MessageId) {
				case 1:
					message = TreeMessage.Deserialize (messageBytes);
					break;
				case 2:
					message = RouteMessage.Deserialize (messageBytes);
					break;
				case 3:
					message = ListMessage.Deserialize (messageBytes);
					break;
				default:
					throw new NotImplementedException ();
				}

				//Verify signature
				if (header.Signature != null) {
					foreach (PublicKey key in keyStorage.PublicKeys) {
						if (key.Verify (messageBytes, header.Signature)) {
							message.Signature = key;
							break;
						}
					}
				}

				return message;
			}

		}

		#endregion

	}
}

