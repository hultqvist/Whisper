using System;
using System.IO;
using System.Collections.Generic;
using Whisper;
using Whisper.Storing;
using Whisper.Blobing;
using Whisper.Messaging;

namespace Test
{
	public class Receiver
	{
		Storage storage;
		string target;
		KeyStorage keyStorage;

		public Receiver(Storage storage, string target, KeyStorage keyStorage)
		{
			this.storage = storage;
			this.target = target;
			this.keyStorage = keyStorage;
		}

		public void Run()
		{
			//Find message
			ICollection<BlobHash> messages = storage.GetMessageList();
			EncryptedStorage es = new EncryptedStorage(storage, keyStorage);

			foreach (BlobHash mid in messages)
			{
				Blob blob = es.ReadBlob(mid);
				Message message = Message.FromBlob(blob, keyStorage);
				if (message == null)
				{
					Console.WriteLine("Missing key: " + mid);
					continue;
				}

				TreeMessage tm = message as TreeMessage;
				if (tm != null)
				{
					Console.WriteLine("Found TreeMessage " + tm.Name);
					string targetPath = Path.Combine(target, tm.Name);
					TreeBlob.Extract(es, tm.TreeID, targetPath);
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

