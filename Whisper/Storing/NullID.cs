using System;
using Whisper.Chunks;

namespace Whisper.Storing
{
	/// <summary>
	/// Only pass the BlobHash
	/// </summary>
	public class NullID : IGenerateID
	{
		public CustomID GetID(Chunk blob)
		{
			return null;
		}
	}
}

