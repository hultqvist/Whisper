using System;
using Whisper.Chunks;

namespace Whisper.Repos
{
	/// <summary>
	/// CustomID generator, used by Storages to add a CustomID to chunks.
	/// CustomID is useful to identify the same data encrypted with different keys to
	/// avoid sending the same data twice.
	/// </summary>
	public interface IGenerateID
	{
		CustomID GetID(Chunk chunk);
	}
}

