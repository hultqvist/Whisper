using System;
namespace Whisper.Messaging
{
	/// <summary>
	/// Signed messages
	/// </summary>
	public abstract class SignedMessage : Message
	{
		public PublicKey Signature { get; set; }
	}
}

