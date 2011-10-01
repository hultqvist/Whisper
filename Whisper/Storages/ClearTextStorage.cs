using System;
using Whisper.Storages;
using Whisper.Chunks;
namespace Whisper.Storages
{
	/// <summary>
	/// Store the data in cleartext
	/// </summary>
	public class ClearTextStorage : StorageFilter
	{
		private readonly IGenerateID clearTextID = new ClearTextID();

		public ClearTextStorage(Storage storage) : base(storage)
		{
		}

		public override void WriteChunk(Chunk chunk)
		{
			chunk.CustomID = clearTextID.GetID(chunk);
			base.WriteChunk(chunk);
		}
	}
}

