using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestWriteChunk : IPipeMessage
	{
		public uint TypeID { get { return 3; } }
	}

	public partial class ReplyWriteChunk
	{
	
	}
}
