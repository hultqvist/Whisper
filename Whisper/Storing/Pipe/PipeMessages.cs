using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Whisper.Storing.Pipe
{
	/// <summary>
	/// This header is sent before every message to determine the type of the message
	/// </summary>
	public partial class PipeHeader
	{
		static uint nextDebug = 1;

		public static PipeHeader Next()
		{
			PipeHeader h = new PipeHeader();
			h.DebugNumber = nextDebug;
			nextDebug++;
			return h;
		}
	}

	public interface IPipeMessage
	{
		uint TypeID { get; }
	}

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

	public partial class RequestReadChunk : IPipeMessage
	{
		public uint TypeID { get { return 2; } }
	}

	public partial class ReplyReadChunk
	{
	}

	public partial class RequestWriteChunk : IPipeMessage
	{
		public uint TypeID { get { return 3; } }
	}

	public partial class ReplyWriteChunk
	{
	
	}

	public partial class RequestMessageList : IPipeMessage
	{
		public uint TypeID { get { return 4; } }
	}

	public partial class ReplyMessageList
	{
	}

	public partial class RequestStoreMessage : IPipeMessage
	{
		public uint TypeID { get { return 5; } }
	}

	public partial class ReplyStoreMessage
	{

	}

}
