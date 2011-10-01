using System;
using Whisper.Keys;

namespace Whisper.Messages
{
	/// <summary>
	/// Signed messages
	/// </summary>
	public interface SignedMessage
	{
		PublicKey Signature { get; set; }
	}
}

