using System;
using System.Collections.Generic;
using Whisper;
using Whisper.Storing;
using Whisper.Messaging;
using System.IO;
namespace Wcp
{
	public static class List
	{
		public static void Main(string[] args)
		{
			if (args.Length != 2)
				throw new HelpException("Missing arguments");

			//Storage
			Storage storage = new Whisper.Storing.DiskStorage(args[1]);

			//Sender and Recipient keys
			KeyStorage keyStorage = new KeyStorage();

			//Find message
			ICollection<BlobHash> messages = storage.GetMessageList();
			EncryptedStorage es = new EncryptedStorage(storage, keyStorage);

			//Iterate over all messages
			foreach (BlobHash mid in messages)
			{
				Console.Write(mid.ToString().Substring(0, 10) + "... ");

				Message message = Message.FromBlob(es.ReadBlob(mid), keyStorage);

				//No key found
				if (message == null)
				{
					Console.WriteLine("no key");
					continue;
				}

				SignedMessage sm = message as SignedMessage;
				if (sm != null && sm.Signature != null)
					Console.Write("signed by " + sm.Signature.ToString().Substring(0, 10) + " ");

				TreeMessage tm = message as TreeMessage;
				if (tm != null)
				{
					Console.WriteLine("TreeMessage " + tm.Name);
					continue;
				}

				RouteMessage rm = message as RouteMessage;
				if (rm != null)
				{
					Console.WriteLine("RouteMessage to " + rm.To);
					continue;
				}

				Console.WriteLine("unknown message type: " + message.GetType().Name);
			}
		}
	}
}

