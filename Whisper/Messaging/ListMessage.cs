using System;
using System.Collections.Generic;
using System.IO;
using Whisper.Blobing;

namespace Whisper.Messaging
{
	/// <summary>
	/// List of multiple messages
	/// If using an encrypted storage this message type can be used to
	/// hide that multiple messages are being sent.
	/// </summary>
	public class ListMessage : SignedMessage
	{
		public List<TrippleID> List = new List<TrippleID>();

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			throw new NotImplementedException();
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			throw new NotImplementedException();
		}
		
		#endregion
	
	}
}

