using System;
using Whisper.Chunks;

namespace Whisper.Storing
{
	/// <summary>
	/// CustomID generator, used in various Storage to add a CustomID to blobs
	/// </summary>
	public interface IGenerateID
	{
		CustomID GetID(Chunk blob);
	}
}

