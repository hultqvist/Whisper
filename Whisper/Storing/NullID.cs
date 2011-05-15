using System;
using Whisper.Blobing;

namespace Whisper.Storing
{
	/// <summary>
	/// Only pass the BlobHash
	/// </summary>
	public class NullID : IGenerateID
	{
		public CustomID GetID(Blob blob)
		{
			return null;
		}
	}
}

