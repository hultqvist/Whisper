using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestStoreMessage : IPipeMessage
	{
		public uint TypeID { get { return 5; } }
	}

	public partial class ReplyStoreMessage
	{

	}
}
