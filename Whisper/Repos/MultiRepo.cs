using System;
using System.Collections.Generic;
using Whisper.Chunks;
namespace Whisper.Repos
{
	/// <summary>
	/// This repo redirects all read and write requests to multiple other repos.
	/// </summary>
	public class MultiRepo : Repo
	{
		readonly IEnumerable<Repo> repos;

		public MultiRepo(IEnumerable<Repo> repoList)
		{
			this.repos = repoList;
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			//foreach (Storage s in repos)
			throw new System.NotImplementedException();
		}


		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			foreach (Repo r in repos)
			{
				Chunk b = r.ReadChunk(chunkHash);
				if (b != null)
					return b;
			}
			return null;
		}


		public override ChunkHash WriteChunk(Chunk chunk)
		{
			ChunkHash ch = null;
			foreach (Repo r in repos)
				ch = r.WriteChunk(chunk);
			return ch;
		}


		public override List<ChunkHash> GetMessageList()
		{
			List<ChunkHash> list = new List<ChunkHash>();
			foreach (Repo r in repos)
			{
				var sl = r.GetMessageList();
				foreach (ChunkHash bh in sl)
					list.Add(bh);
			}
			return list;
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			foreach (Repo r in repos)
				r.StoreMessage(chunkHash);
		}

	}
}

