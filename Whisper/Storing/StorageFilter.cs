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

		public override string ToString ()
		{
			return backend.ToString();
		}

		public override ChunkHash GetCustomHash(CustomID id)
		{
			return backend.GetCustomHash(id);
		}

		public override void WriteChunk(Chunk chunk)
		{
			backend.WriteChunk(chunk);
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			return backend.ReadChunk(chunkHash);
		}

		public override List<ChunkHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			backend.StoreMessage(chunkHash);
		}

	}
}

