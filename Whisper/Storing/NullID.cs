using System;
using Whisper.Chunks;

namespace Whisper.Storing
{
	/// <summary>
	/// No custom ID
	/// </summary>
	public class NullID : IGenerateID
	{
		public CustomID GetID(Chunk chunk)
		{
			return null;
		}
	}
}

