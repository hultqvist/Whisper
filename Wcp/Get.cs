using System;
using System.Collections.Generic;
using Whisper;
using Whisper.Storing;

namespace Wcp
{
	public static class Get
	{
		public static void Main (string[] args)
		{
			if (args.Length != 4)
				throw new HelpException ("Missing arguments");
			
			//Storage
			Storage storage = new Whisper.Storing.DiskStorage (args[1]);
			
			//Sender and Recipient keys
			KeyStorage keyStorage = new KeyStorage ();
			
			//Find message
			ICollection<BlobHash> messages = storage.GetMessageList();
			EncryptedStorage es = new EncryptedStorage(storage, keyStorage, new RecipientID(keyStorage.DefaultKey));

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
					TreeBlob.Extract(es, fm.TreeID, targetPath);
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
			throw new HelpException("Not Implemented");
		}
	}
}

