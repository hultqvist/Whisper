using System;
using Whisper.Blobs;
using System.Collections.Generic;
using Whisper.Messages;
namespace Whisper.Storages
{
	public class StorageFilter : Storage
	{
		private readonly Storage backend;
		public StorageFilter(Storage storage)
		{
			this.backend = storage;
		}

		public override BlobHash GetBlobHash(CustomID id)
		{
			return backend.GetBlobHash(id);
		}

		public override void WriteBlob(Blob blob)
		{
			backend.WriteBlob(blob);
		}

		public override Blob ReadBlob(BlobHash blobHash)
		{
			return backend.ReadBlob(blobHash);
		}

		public override List<BlobHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(BlobHash blobHash)
		{
			backend.StoreMessage(blobHash);
		}

	}
}

