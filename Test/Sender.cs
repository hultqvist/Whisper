using System;
using System.Collections.Generic;
using Whisper;
using Whisper.Blobing;
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
			Blob treeBlob = TreeBlob.GenerateBlob(source, storage, blobList);
			TreeMessage fm = new TreeMessage(treeBlob.ClearID, "EncryptedTest");
			Blob fc = SignedMessage.ToBlob(fm, keyStorage.DefaultKey);
			storage.WriteBlob(fc);
			blobList.Add(fc.BlobHash);
			//storage.StoreMessage(fc.BlobHash);
			return fc;
		}

		private void TestRouteMessage(EncryptedStorage storage, Blob messageBlob, List<BlobHash> blobList)
		{
			RouteMessage rm = new RouteMessage("StorageB", messageBlob.BlobHash, blobList.ToArray());
			Blob blob = Message.ToBlob(rm, keyStorage.DefaultKey);
			storage.WriteBlob(blob);
			storage.StoreMessage(blob.BlobHash);
		}

		private void TestClearTextTree(ClearTextStorage storage)
		{
			Blob tree = TreeBlob.GenerateBlob(source, storage, null);
			TreeMessage clearFM = new TreeMessage(tree.ClearID, "ClearTextTest");
			Blob ctMessageBlob = Message.ToBlob(clearFM, null);
			storage.WriteBlob(ctMessageBlob);
			storage.StoreMessage(ctMessageBlob.BlobHash);
		}
	}
}

