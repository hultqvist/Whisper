﻿//
//	You may customize this code as you like
//	Report bugs to: https://silentorbit.com/protobuf/
//
//	Generated by ProtocolBuffer
//	- a pure c# code generation implementation of protocol buffers
//

using System;
using System.Collections.Generic;

namespace Whisper.Chunks
{
	public partial class ChunkKeys
	{
		//public byte[] IV { get; set; }	//Implemented by user elsewhere
		public List<byte[]> EncryptedKeys { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
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
		public byte[] ChunkData { get; set; }
		public Whisper.Chunks.ChunkKeys Keys { get; set; }
	}

}
namespace Whisper.Repos.Pipe
{
	public partial class RequestWriteChunk
	{
		public byte[] ChunkData { get; set; }
		public Whisper.Chunks.ChunkKeys Keys { get; set; }
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
