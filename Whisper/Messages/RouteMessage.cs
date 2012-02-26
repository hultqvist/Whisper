using System;
using System.Collections.Generic;
using System.IO;
using Whisper.Chunks;

namespace Whisper.Messages
{
	public partial class RouteMessage : Message
	{
		public RouteMessage ()
		{
			this.Chunks = new List<byte[]> ();
		}
	}
}

