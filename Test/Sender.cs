using System;
using System.Collections.Generic;
using Whisper;
using Whisper.Chunks;
using Whisper.Storing;
using Whisper.Messaging;

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
			List<ChunkHash> blobList = new List<ChunkHash>();
			Console.WriteLine("Send directory encrypted " + source);
			Chunk blobTreeMessage = TestEncryptedTree(encStorage, blobList);

			//Route message
			Console.WriteLine("Send route message...");
			TestRouteMessage(encStorage, blobTreeMessage, blobList);

			//TreeMessage ClearText
			TestClearTextTree(new ClearTextStorage(storage));
			Console.WriteLine("Send tree unencrypted...");
		}

		private Chunk TestEncryptedTree(EncryptedStorage storage, List<ChunkHash> blobList)
		{
			Chunk treeBlob = TreeChunk.GenerateBlob(source, storage, blobList);
			TreeMessage fm = new TreeMessage(treeBlob.ClearID, "EncryptedTest");
			Chunk fc = SignedMessage.ToChunk(fm, keyStorage.DefaultKey);
			storage.WriteChunk(fc);
			blobList.Add(fc.ChunkHash);
			//storage.StoreMessage(fc.BlobHash);
			return fc;
		}

		private void TestRouteMessage(EncryptedStorage storage, Chunk messageBlob, List<ChunkHash> blobList)
		{
			RouteMessage rm = new RouteMessage("StorageB", messageBlob.ChunkHash, blobList.ToArray());
			Chunk blob = Message.ToChunk(rm, keyStorage.DefaultKey);
			storage.WriteChunk(blob);
			storage.StoreMessage(blob.ChunkHash);
		}

		private void TestClearTextTree(ClearTextStorage storage)
		{
			Chunk tree = TreeChunk.GenerateBlob(source, storage, null);
			TreeMessage clearFM = new TreeMessage(tree.ClearID, "ClearTextTest");
			Chunk ctMessageChunk = Message.ToChunk(clearFM, null);
			storage.WriteChunk(ctMessageChunk);
			storage.StoreMessage(ctMessageChunk.ChunkHash);
		}
	}
}

