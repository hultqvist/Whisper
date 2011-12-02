using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	public partial class RequestCustomHash : IPipeMessage
	{
		public uint TypeID { get { return 1; } }

		public override string ToString()
		{
			return "RequestCustomHash: " + BitConverter.ToString(this.CustomID);
		}
	}

	public partial class ReplyCustomHash
	{
		public override string ToString()
		{
			if (ChunkHash == null)
				return "ReplyCustomHash: null";
			return "ReplyCustomHash: " + BitConverter.ToString(this.ChunkHash);
		}
	}
}
