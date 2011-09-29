using System;
using Whisper.Keys;

namespace Whisper.Messaging
{
	/// <summary>
	/// Signed messages
	/// </summary>
	public interface SignedMessage
	{
		PublicKey Signature { get; set; }
	}
}

