using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestReadChunk : IPipeMessage
	{
		public uint TypeID { get { return 2; } }
	}

	public partial class ReplyReadChunk
	{
	}
}
