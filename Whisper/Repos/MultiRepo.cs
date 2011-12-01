using System;
using System.Collections.Generic;
using Whisper.Chunks;
namespace Whisper.Repos
{
	/// <summary>
	/// This storage redirects all read and write requests to multiple other storages.
	/// </summary>
	public class MultiRepo : Repo
	{
		readonly IEnumerable<Repo> storages;

		public MultiRepo(IEnumerable<Repo> storages)
		{
			this.storages = storages;
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			//foreach (Storage s in storages)
			throw new System.NotImplementedException();
		}


		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			foreach (Repo s in storages)
			{
				Chunk b = s.ReadChunk(chunkHash);
				if (b != null)
					return b;
			}
			return null;
		}


		public override void WriteChunk(Chunk chunk)
		{
			foreach (Repo s in storages)
				s.WriteChunk(chunk);
		}


		public override List<ChunkHash> GetMessageList()
		{
			List<ChunkHash> list = new List<ChunkHash>();
			foreach (Repo s in storages)
			{
				var sl = s.GetMessageList();
				foreach (ChunkHash bh in sl)
					list.Add(bh);
			}
			return list;
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			foreach (Repo s in storages)
				s.StoreMessage(chunkHash);
		}

	}
}

