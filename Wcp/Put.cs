using System;
using System.IO;
using Whisper;
using Whisper.Messages;
using Whisper.Keys;

namespace Wcp
{
	/// <summary>
	/// Store a lokal file tree into a repo
	/// </summary>
	public static class Put
	{
		public static void Main(string[] args, KeyStorage keyStorage)
		{
			//Usage
			if (args.Length != 4)
				throw new HelpException("Missing arguments");
			string sourcePath = args[1];
			string storagePath = args[2];
			string receipientName = args[3];
			
			//Source
			if (Directory.Exists(sourcePath) == false)
				throw new HelpException("Source directory not found: " + sourcePath);
			
			//Storage
			Repo storage = Repo.Create(storagePath);
			
			//Sender and Recipient keys
			PrivateKey senderKey = keyStorage.DefaultKey;
			PublicKey recipientKey = keyStorage.GetPublic(receipientName);
			
			//Send Tree
			Console.Write("Generating Tree...");
			Tree tree = new Tree();
			tree.SourcePath = sourcePath;
			tree.TargetName = Path.GetDirectoryName(sourcePath);
			tree.Repo.Add(storage);
			tree.EncryptKeys.Add(recipientKey);
			tree.SigningKey = senderKey;
			ChunkHash treeMessage = tree.Generate();
			Console.WriteLine("done");
			
			//Store Message ID
			storage.StoreMessage(treeMessage);
			
			//RouteMessage
			RouteMessage rm = new RouteMessage();
			rm.MessageChunkHash = treeMessage.bytes;
			foreach (ChunkHash ch in tree.ChunkList)
				rm.Chunks.Add(ch.bytes);
			rm.To = receipientName;
			//Store unencrypted RouteMessage
			Whisper.Chunks.Chunk rmChunk = Message.ToChunk(rm);
			storage.WriteChunk(rmChunk);
			storage.StoreMessage(rmChunk.ChunkHash);
			Console.WriteLine("RouteMessage Stored");
			
		}
	}
}

