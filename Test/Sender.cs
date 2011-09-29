using System;
using System.Collections.Generic;
using Whisper;
using Whisper.Chunks;
using Whisper.Storing;
using Whisper.Messaging;
using Whisper.Keys;

namespace Test
{
	public class Sender
	{
		string source;
		Storage storage;
		KeyStorage keyStorage;
		PublicKey targetKey;

		public Sender(string source, Storage storage, KeyStorage keyStorage, PublicKey targetKey)
		{
			this.source = source;
			this.storage = storage;
			this.keyStorage = keyStorage;
			this.targetKey = targetKey;
		}

		public void Run()
		{
			Console.WriteLine("Create Encrypted storage...");
			EncryptedStorage encStorage = new EncryptedStorage(storage, null, new RecipientID(targetKey));
			encStorage.AddKey(targetKey);
			
			//TreeMessage Encrypted
			List<ChunkHash> chunkList = new List<ChunkHash>();
			Console.WriteLine("Send directory encrypted " + source);
			Chunk chunkTreeMessage = TestEncryptedTree(encStorage, chunkList);
			
			//Route message
			Console.WriteLine("Send route message...");
			TestRouteMessage(encStorage, chunkTreeMessage, chunkList);
			
			//TreeMessage ClearText
			TestClearTextTree(new ClearTextStorage(storage));
			Console.WriteLine("Send tree unencrypted...");
		}

		private Chunk TestEncryptedTree(EncryptedStorage storage, List<ChunkHash> chunkList)
		{
			Chunk treeChunk = TreeChunk.GenerateChunk(source, storage, chunkList);
			TreeMessage fm = new TreeMessage(treeChunk.TrippleID, "EncryptedTest");
			Chunk fc = Message.ToChunk(fm, keyStorage.DefaultKey);
			storage.WriteChunk(fc);
			chunkList.Add(fc.DataHash);
			//storage.StoreMessage(fc.ChunkHash);
			return fc;
		}

		private void TestRouteMessage(EncryptedStorage storage, Chunk messageChunk, List<ChunkHash> chunkList)
		{
			RouteMessage rm = new RouteMessage();
			rm.To = "StorageB";
			rm.MessageChunkHash = messageChunk.DataHash.bytes;
			foreach (ChunkHash ch in chunkList)
				rm.Chunks.Add(ch.bytes);
			Chunk chunk = Message.ToChunk(rm, keyStorage.DefaultKey);
			storage.WriteChunk(chunk);
			storage.StoreMessage(chunk.DataHash);
		}

		private void TestClearTextTree(ClearTextStorage storage)
		{
			Chunk tree = TreeChunk.GenerateChunk(source, storage, null);
			TreeMessage clearFM = new TreeMessage(tree.TrippleID, "ClearTextTest");
			Chunk ctMessageChunk = Message.ToChunk(clearFM, null);
			storage.WriteChunk(ctMessageChunk);
			storage.StoreMessage(ctMessageChunk.DataHash);
		}
	}
}

