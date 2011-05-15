using System;
using Whisper.Blobing;
using System.Collections.Generic;
using Whisper.Messaging;
namespace Whisper.Storing
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

		public override ICollection<BlobHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(BlobHash blobHash)
		{
			backend.StoreMessage(blobHash);
		}

	}
}

