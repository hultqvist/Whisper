using System;
using Whisper.Chunks;

namespace Whisper.Chunks.ID
{
	/// <summary>
	/// No custom ID.
	/// </summary>
	public class NullID : IGenerateID
	{
		public CustomID GetID(Chunk chunk)
		{
			return null;
		}
	}
}

