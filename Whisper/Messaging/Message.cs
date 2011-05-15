using System;
using System.IO;
using Whisper.Blobing;
using System.Text;
using System.Collections.Generic;
namespace Whisper.Messaging
{
	/// <summary>
	/// Unsigned messages
	/// </summary>
	public abstract class Message : BinaryBlob
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

			throw new ArgumentException("ID not found");
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

		#region Static Blob Readers and Writers

		public static Blob ToBlob(Message m)
		{
			return ToBlob(m, null);
		}

		public static Blob ToBlob(Message m, PrivateKey signatureKey)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				//Message type
				BinaryWriter b = new BinaryWriter(ms);
				b.Write(Message.GetID(m));
				
				//Message Data
				m.WriteBlob(new BinaryWriter(ms));
				
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
				
				//Generate Blob Data
				byte[] data = ms.ToArray();
				return new Blob(data);
			}
		}

		public static Message FromBlob(Blob blob)
		{
			return FromBlob(blob, null);
		}
		public static Message FromBlob(Blob blob, KeyStorage keyStorage)
		{
			int typeID = BitConverter.ToInt32(blob.Data, 0);
			Message message = Message.FromID(typeID);
			
			//all but last signature
			byte[] data;
			if (message is SignedMessage)
			{
				data = new byte[blob.Data.Length - 128];
				Array.Copy(blob.Data, 0, data, 0, data.Length);
			} else
				data = blob.Data;
			
			//Message Data
			using (MemoryStream ms = new MemoryStream(data))
			{
				BinaryReader br = new BinaryReader(ms);
				br.ReadInt32();
				message.ReadBlob(br);
			}

			//Verify signature
			if (message is SignedMessage)
			{
				byte[] sig = new byte[128];
				Array.Copy(blob.Data, blob.Data.Length - sig.Length, sig, 0, sig.Length);
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

