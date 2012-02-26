using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestMessageList : IPipeMessage
	{
		/// <summary>
		/// Only for internal use in serializer
		/// </summary>
		public RequestMessageList ()
		{
		}

		public RequestMessageList (string prefix)
		{
			this.Prefix = prefix;
		}

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
