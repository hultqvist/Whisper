using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestMessageList : IPipeMessage
	{
		public uint TypeID { get { return 4; } }
	}

	public partial class ReplyMessageList
	{
		public ReplyMessageList ()
		{
			this.ChunkHash = new List<byte[]> ();	
		}
	}
}
