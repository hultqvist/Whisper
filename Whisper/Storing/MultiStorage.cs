using System;
using System.Collections.Generic;
using Whisper.Chunks;
namespace Whisper.Storing
{
	public class MultiStorage : Storage
	{
		readonly IEnumerable<Storage> storages;

		public MultiStorage(IEnumerable<Storage> storages)
		{
			this.storages = storages;
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			foreach (Storage s in storages)
				throw new System.NotImplementedException();
			throw new System.NotImplementedException();
		}


		public override Chunk ReadChunk(ChunkHash blobHash)
		{
			foreach (Storage s in storages)
			{
				Chunk b = s.ReadChunk(blobHash);
				if (b != null)
					return b;
			}
			return null;
		}


		public override void WriteChunk(Chunk blob)
		{
			foreach (Storage s in storages)
				s.WriteChunk(blob);
		}


		public override ICollection<ChunkHash> GetMessageList()
		{
			List<ChunkHash> list = new List<ChunkHash>();
			foreach (Storage s in storages)
			{
				var sl = s.GetMessageList();
				foreach (ChunkHash bh in sl)
					list.Add(bh);
			}
			return list;
		}

		public override void StoreMessage(ChunkHash blobHash)
		{
			foreach (Storage s in storages)
				s.StoreMessage(blobHash);
		}

	}
}

