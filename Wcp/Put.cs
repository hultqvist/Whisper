using System;
using System.IO;
using Whisper;
using Whisper.Messages;
using Whisper.Encryption;
using Whisper.Repos;
using Whisper.Chunks;
using Whisper.ChunkGenerator;

namespace Wcp
{
	/// <summary>
	/// Store a lokal file tree into a repo
	/// </summary>
	public static class Put
	{
		public static void Main (string[] args, KeyStorage keyStorage)
		{
			//Usage
			if (args.Length != 4)
				throw new HelpException ("Missing arguments");
			string sourcePath = args [1];
			string repoPath = args [2];
			string receipientName = args [3];
			
			//Source
			if (Directory.Exists (sourcePath) == false)
				throw new HelpException ("Source directory not found: " + sourcePath);

			//Repo
			Repo repo = Repo.Create (repoPath);

			//Sender and Recipient keys
			PrivateKey senderKey = keyStorage.DefaultKey;
			PublicKey recipientKey = keyStorage.GetPublic (receipientName);

			//Prepare Route message recording of ChunkID
			RouteRepo rr = new RouteRepo (repo);

			//Prepare Encryption
			EncryptedRepo er = new EncryptedRepo (rr, null);
			er.AddKey (recipientKey);

			Console.Write ("Generating Tree...");

			//Send Tree
			ChunkHash tree = TreeChunk.GenerateChunk (sourcePath, er);

			//TreeMessage
			TreeMessage tm = new TreeMessage (tree, Path.GetDirectoryName (sourcePath));
			Chunk tmc = Message.ToChunk (tm, senderKey);
			ChunkHash tmch = er.WriteChunk (tmc);
			er.StoreMessage ("file", tmch);

			//RouteMessage
			RouteMessage rm = rr.RouteMessage;
			rm.MessageChunkHash = tmch.bytes;
			rm.To = receipientName;

			//Store unencrypted RouteMessage
			Chunk rmChunk = Message.ToChunk (rm);
			repo.WriteChunk (rmChunk);
			repo.StoreMessage ("route", rmChunk.ChunkHash);
			Console.WriteLine ("RouteMessage Stored");
		}
	}
}

