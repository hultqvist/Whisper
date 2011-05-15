using System;
using System.Collections.Generic;
using Whisper.Blobing;
namespace Whisper.Storing
{
	public class MultiStorage : Storage
	{
		readonly IEnumerable<Storage> storages;

		public MultiStorage(IEnumerable<Storage> storages)
		{
			this.storages = storages;
		}

		public override BlobHash GetBlobHash(CustomID customID)
		{
			foreach (Storage s in storages)
				throw new System.NotImplementedException();
			throw new System.NotImplementedException();
		}


		public override Blob ReadBlob(BlobHash blobHash)
		{
			foreach (Storage s in storages)
			{
				Blob b = s.ReadBlob(blobHash);
				if (b != null)
					return b;
			}
			return null;
		}


		public override void WriteBlob(Blob blob)
		{
			foreach (Storage s in storages)
				s.WriteBlob(blob);
		}


		public override ICollection<BlobHash> GetMessageList()
		{
			List<BlobHash> list = new List<BlobHash>();
			foreach (Storage s in storages)
			{
				var sl = s.GetMessageList();
				foreach (BlobHash bh in sl)
					list.Add(bh);
			}
			return list;
		}

		public override void StoreMessage(BlobHash blobHash)
		{
			foreach (Storage s in storages)
				s.StoreMessage(blobHash);
		}

	}
}

