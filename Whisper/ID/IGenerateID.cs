using System;
using Whisper.Blobs;

namespace Whisper.ID
{
	/// <summary>
	/// CustomID generator, used in various Storage to add a CustomID to blobs
	/// </summary>
	public interface IGenerateID
	{
		CustomID GetID(Blob blob);
	}
}

