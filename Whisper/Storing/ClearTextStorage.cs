using System;
using Whisper.Storing;
using Whisper.Chunks;
namespace Whisper.Storing
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

		public override void WriteChunk(Chunk blob)
		{
			blob.CustomID = clearTextID.GetID(blob);
			base.WriteChunk(blob);
		}
	}
}

