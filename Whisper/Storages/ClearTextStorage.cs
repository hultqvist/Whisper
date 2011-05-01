using System;
using Whisper.ID;
using Whisper.Blobs;
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

		public override void WriteBlob(Blob blob)
		{
			blob.CustomID = clearTextID.GetID(blob);
			base.WriteBlob(blob);
		}
	}
}

