﻿//
//	You may customize this code as you like
//	Report bugs to: https://silentorbit.com/protobuf/
//
//	Generated by ProtocolBuffer
//	- a pure c# code generation implementation of protocol buffers
//

using System;
using System.Collections.Generic;

namespace Whisper.Repos.Pipe
{
	/// <summary>
	/// Message structures for the PipeRepo protocol
	/// </summary>
	public partial class PipeHeader
	{
		public uint TypeID { get; set; }
		public uint DebugNumber { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestCustomHash
	{
		public byte[] CustomID { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class ReplyCustomHash
	{
		public byte[] ChunkHash { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestReadChunk
	{
		public byte[] ChunkHash { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class ReplyReadChunk
	{
		/// <summary>
		/// Only set if found
		/// </summary>
		public byte[] ChunkData { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestWriteChunk
	{
		public byte[] ChunkData { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class ReplyWriteChunk
	{
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestMessageList
	{
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class ReplyMessageList
	{
		public List<byte[]> ChunkHash { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestStoreMessage
	{
		public byte[] ChunkHash { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class ReplyStoreMessage
	{
	}

}
