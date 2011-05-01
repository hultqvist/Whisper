using System;
using System.Collections.Generic;
using Whisper.Blobs;
using Whisper.Messages;
using Whisper.Storages;
using Whisper.ID;

namespace Whisper
{
	public class Sender
	{
		string source;
		Storage storage;
		PrivateKey key;
		PublicKey targetKey;

		public Sender(string source, Storage storage, PrivateKey key, PublicKey targetKey)
		{
			this.source = source;
			this.storage = storage;
			this.key = key;
			this.targetKey = targetKey;
		}

		public void Run()
		{
			Console.WriteLine("Create Encrypted storage...");
			EncryptedStorage encStorage = new EncryptedStorage(storage, new RecipientID(targetKey));
			encStorage.AddKey(targetKey);

			//TreeMessage Encrypted
			List<BlobHash> blobList = new List<BlobHash>();
			Console.WriteLine("Send directory encrypted " + source);
			Blob blobTreeMessage = TestEncryptedTree(encStorage, blobList);

			//Route message
			Console.WriteLine("Send route message...");
			TestRouteMessage(encStorage, blobTreeMessage, blobList);

			//TreeMessage ClearText
			TestClearTextTree(new ClearTextStorage(storage));
			Console.WriteLine("Send tree unencrypted...");
		}

		private Blob TestEncryptedTree(EncryptedStorage storage, List<BlobHash> blobList)
		{
			Blob treeBlob = Tree.GenerateBlob(source, storage, blobList);
			TreeMessage fm = new TreeMessage(treeBlob.ClearID, "EncryptedTest");
			Blob fc = SignedMessage.ToBlob(fm, key);
			storage.WriteBlob(fc);
			blobList.Add(fc.BlobHash);
			//storage.StoreMessage(fc.BlobHash);
			return fc;
		}

		private void TestRouteMessage(EncryptedStorage storage, Blob messageBlob, List<BlobHash> blobList)
		{
			RouteMessage rm = new RouteMessage("StorageB", messageBlob.BlobHash, blobList.ToArray());
			Blob blob = Message.ToBlob(rm, key);
			storage.WriteBlob(blob);
			storage.StoreMessage(blob.BlobHash);
		}

		private void TestClearTextTree(ClearTextStorage storage)
		{
			Blob tree = Tree.GenerateBlob(source, storage, null);
			TreeMessage clearFM = new TreeMessage(tree.ClearID, "ClearTextTest");
			Blob ctMessageBlob = Message.ToBlob(clearFM, null);
			storage.WriteBlob(ctMessageBlob);
			storage.StoreMessage(ctMessageBlob.BlobHash);
		}
	}
}

