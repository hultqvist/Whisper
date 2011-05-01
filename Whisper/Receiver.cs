using System;
using System.Collections.Generic;
using Whisper.Storages;
using Whisper.Blobs;
using Whisper.Messages;
using System.IO;
namespace Whisper
{
	public class Receiver
	{
		Storage storage;
		string target;
		Key key;
		Key senderKey;

		public Receiver(Storage storage, string target, Key key, Key senderKey)
		{
			this.storage = storage;
			this.target = target;
			this.key = key;
			this.senderKey = senderKey;
		}

		public void Run()
		{
			//Find message
			List<BlobHash> messages = storage.GetMessageList();
			EncryptedStorage es = new EncryptedStorage(storage, null);
			es.AddKey(key);
			es.AddKey(senderKey);

			foreach (BlobHash mid in messages)
			{
				Blob blob = es.ReadBlob(mid);
				Message message = Message.FromBlob(blob, senderKey);
				if (message == null)
				{
					Console.WriteLine("Missing key: " + mid);
					continue;
				}

				TreeMessage fm = message as TreeMessage;
				if (fm != null)
				{
					Console.WriteLine("Found TreeMessage " + fm.Name);
					string targetPath = Path.Combine(target, fm.Name);
					Tree.Extract(es, fm.TreeID, targetPath);
					continue;
				}

				RouteMessage rm = message as RouteMessage;
				if (rm != null)
				{
					Console.WriteLine("Found RouteMessage to " + rm.To);

					//Prepare new storage
					Storage remoteStorage = new DiskStorage(rm.To);

					//Send blobs
					foreach (BlobHash blobHash in rm.Blobs)
					{
						Blob b = storage.ReadBlob(blobHash);
						remoteStorage.WriteBlob(b);
					}

					//Send message
					remoteStorage.StoreMessage(rm.Message);
					continue;
				}

				Console.WriteLine("Unhandled message type: " + message.GetType().Name);
			}

		}

	}
}

