using System;
using Whisper.Chunks;
using System.Collections.Generic;

namespace Whisper.Repos
{
	/// <summary>
	/// Helper for filters that only intercept a few commands
	/// </summary>
	public abstract class RepoFilter : Repo
	{
		Repo backend;

		public RepoFilter(Repo backendRepo)
		{
			this.backend = backendRepo;
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			return backend.ReadChunk(chunkHash);
		}

		public override ChunkHash WriteChunk(Chunk chunk)
		{
			return backend.WriteChunk(chunk);
		}

		public override List<ChunkHash> GetMessageList()
		{
			return backend.GetMessageList();
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			backend.StoreMessage(chunkHash);
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			return backend.GetCustomHash(customID);
		}
	}
}

