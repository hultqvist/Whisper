using System;
using Whisper.Blobing;
namespace Whisper.Storing
{
	/// <summary>
	/// Generates CustomID that equals the cleartext hash
	/// </summary>
	public class ClearTextID : IGenerateID
	{
		public CustomID GetID(Blob blob)
		{
			return new CustomID(blob.ClearHash);
		}
	}
}

