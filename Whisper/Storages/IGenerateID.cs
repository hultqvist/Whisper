using System;
using Whisper.Chunks;

namespace Whisper.Storages
{
	/// <summary>
	/// CustomID generator, used in various Storage to add a CustomID to chunks
	/// </summary>
	public interface IGenerateID
	{
		CustomID GetID(Chunk chunk);
	}
}

