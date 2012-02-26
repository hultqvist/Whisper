using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	/// <summary>
	/// This header is sent before every message to determine the type of the message
	/// </summary>
	public partial class PipeHeader
	{
		static uint nextDebug = 1;

		public static PipeHeader Next ()
		{
			PipeHeader h = new PipeHeader ();
			h.DebugNumber = nextDebug;
			nextDebug++;
			return h;
		}
	}
}
