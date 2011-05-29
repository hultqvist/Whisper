using System;
using System.IO;
using Whisper.Chunks;
using System.Text;
using System.Collections.Generic;
namespace Whisper.Messaging
{
	/// <summary>
	/// Unsigned messages
	/// </summary>
	public abstract class Message : BinaryChunk
	{
		#region Message Type

		public static Message FromID(int id)
		{
			if (id == 1)
				return new TreeMessage();
			if (id == 2)
				return new RouteMessage();
			if (id == 3)
				return new ListMessage();

			//MessageID not recognized
			return null;
		}

		public static int GetID(Message message)
		{
			if (message is TreeMessage)
				return 1;
			if (message is RouteMessage)
				return 2;
			if (message is ListMessage)
				return 3;

			throw new ArgumentException("Unknown message type");
		}

		#endregion

		#region Static Chunk Readers and Writers

		public static Chunk ToChunk(Message m)
		{
			return ToChunk(m, null);
		}

		public static Chunk ToChunk(Message m, PrivateKey signatureKey)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				//Message type
				BinaryWriter b = new BinaryWriter(ms);
				b.Write(Message.GetID(m));
				
				//Message Data
				m.WriteChunk(new BinaryWriter(ms));
				
				//Signature
				if (m is SignedMessage)
				{
					byte[] signature;
					if (signatureKey == null)
						signature = new byte[128];
					else
						signature = signatureKey.Sign(ms.ToArray());
					ms.Write(signature, 0, 128);
				}
				
				//Generate Chunk Data
				byte[] data = ms.ToArray();
				return new Chunk(data);
			}
		}

		public static Message FromChunk(Chunk chunk)
		{
			return FromChunk(chunk, null);
		}
		public static Message FromChunk(Chunk chunk, KeyStorage keyStorage)
		{
			if (chunk == null)
				return null;
			
			int typeID = BitConverter.ToInt32(chunk.Data, 0);
			Message message = Message.FromID(typeID);
			if (message == null)
				return null;

			//all but last signature
			byte[] data;
			if (message is SignedMessage)
			{
				data = new byte[chunk.Data.Length - 128];
				Array.Copy(chunk.Data, 0, data, 0, data.Length);
			} else
				data = chunk.Data;
			
			//Message Data
			using (MemoryStream ms = new MemoryStream(data))
			{
				BinaryReader br = new BinaryReader(ms);
				br.ReadInt32();
				message.ReadChunk(br);
			}

			//Verify signature
			if (message is SignedMessage)
			{
				byte[] sig = new byte[128];
				Array.Copy(chunk.Data, chunk.Data.Length - sig.Length, sig, 0, sig.Length);
				foreach (PublicKey key in keyStorage.PublicKeys)
				{
					if (key.Verify(data, sig))
					{
						((SignedMessage) message).Signature = key;
						break;
					}
				}
			}
			
			return message;
		}
		
		#endregion
		
	}
}

