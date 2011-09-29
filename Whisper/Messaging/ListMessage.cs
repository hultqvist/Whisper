using System;
using System.Collections.Generic;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messaging
{
	/// <summary>
	/// List of multiple messages
	/// If using an encrypted storage this message type can be used to
	/// hide that multiple messages are being sent.
	/// </summary>
	public partial class ListMessage : Message
	{
	}
}

