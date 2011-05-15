using System;
using System.IO;
using Whisper;
using Whisper.Messaging;
namespace Wcp
{
	public static class Put
	{
		public static void Main(string[] args)
		{
			//Usage
			if (args.Length != 3)
				throw new HelpException("Missing arguments");
			string sourcePath = args[0];
			string storagePath = args[1];
			string receipientName = args[2];
			
			//Source
			if (Directory.Exists(sourcePath) == false)
				throw new HelpException("Source directory not found: " + sourcePath);

			//Storage
			Storage storage = new Whisper.Storing.DiskStorage(storagePath);
			
			//Sender and Recipient keys
			KeyStorage keyStorage = new KeyStorage();
			PrivateKey senderKey = keyStorage.DefaultKey;
			PublicKey recipientKey = (PublicKey)keyStorage.FromName(receipientName);
			
			//Send Tree
			Console.Write("Generating Tree...");
			Tree tree = new Tree();
			tree.SourcePath = sourcePath;
			tree.TargetName = Path.GetFileName(sourcePath);
			tree.Storage.Add(storage);
			tree.EncryptKeys.Add(recipientKey);
			tree.SigningKey = senderKey;
			BlobHash treeMessage = tree.Generate();
			Console.WriteLine("done");
			
			//Store Message ID
			storage.StoreMessage(treeMessage);
			
			//RouteMessage
			RouteMessage rm = new RouteMessage();
			rm.Blobs = tree.BlobList;
			rm.To = receipientName;
			//Store unencrypted RouteMessage
			storage.WriteBlob(SignedMessage.ToBlob(rm, senderKey));
			Console.WriteLine("RouteMessage Stored");

		}
	}
}

