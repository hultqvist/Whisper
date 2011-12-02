using System;
using Whisper.Chunks;

namespace Whisper.Chunks.ID
{
	/// <summary>
	/// CustomID generator, used by Repo classes to add a CustomID to chunks.
	/// CustomID is useful to identify the same data encrypted with different keys to
	/// avoid sending the same data twice.
	/// </summary>
	public interface IGenerateID
	{
		CustomID GetID(Chunk chunk);
	}
}

