using System;
using Whisper.Storages;
using Whisper.Chunks;

namespace Whisper.Storages
{
	/// <summary>
	/// Store the data in cleartext
	/// </summary>
	public class ClearTextStorageX : StorageFilter
	{
		private readonly IGenerateID clearTextID = new ClearTextID();

		public ClearTextStorageX(Storage storage) : base(storage)
		{
		}

		public override void WriteChunk(Chunk chunk)
		{
			chunk.CustomID = clearTextID.GetID(chunk);
			base.WriteChunk(chunk);
		}
	}
}

