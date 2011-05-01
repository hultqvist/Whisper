using System;
namespace Whisper.Messages
{
	/// <summary>
	/// Signed messages
	/// </summary>
	public abstract class SignedMessage : Message
	{
		public Key Signature { get; set; }
	}
}

