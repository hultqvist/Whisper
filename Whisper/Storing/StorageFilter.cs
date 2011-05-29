using System;
using Whisper.Chunks;
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

		public override ChunkHash GetCustomHash(CustomID id)
		{
			return backend.GetCustomHash(id);
		}

		public override void WriteChunk(Chunk blob)
		{
			backend.WriteChunk(blob);
		}

		public override Chunk ReadChunk(ChunkHash blobHash)
		{
			return backend.ReadChunk(blobHash);
		}

		public override ICollection<ChunkHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(ChunkHash blobHash)
		{
			backend.StoreMessage(blobHash);
		}

	}
}

