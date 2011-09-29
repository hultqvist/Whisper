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
	public partial class TrippleID
	{
		protected byte[] ChunkHashBytes { get; set; }
		protected byte[] CustomIdBytes { get; set; }
		protected byte[] ClearHashBytes { get; set; }
	
		//protected virtual void BeforeSerialize() {}
		//protected virtual void AfterDeserialize() {}
	}

}
namespace Whisper.Chunks
{
	public partial class StreamChunk
	{
		public ulong Size { get; set; }
		public List<Whisper.Chunks.TrippleID> Chunks { get; set; }
	}

}
namespace Whisper.Chunks
{
	public partial class TreeFile
	{
		public string Name { get; set; }
		public Whisper.Chunks.TrippleID TreeChunkID { get; set; }
	}

}
namespace Whisper.Chunks
{
	public partial class TreeChunk
	{
		public List<Whisper.Chunks.TreeFile> Directories { get; set; }
		public List<Whisper.Chunks.TreeFile> Files { get; set; }
	}

}
namespace Whisper.Messaging
{
	public partial class MessageHeader
	{
		public uint MessageId { get; set; }
		public byte[] Signature { get; set; }
	}

}
namespace Whisper.Messaging
{
	public partial class TreeMessage
	{
		public string Name { get; set; }
		public Whisper.Chunks.TrippleID TreeChunkID { get; set; }
	}

}
namespace Whisper.Messaging
{
	public partial class RouteMessage
	{
		public byte[] MessageChunkHash { get; set; }
		public string To { get; set; }
		public List<byte[]> Chunks { get; set; }
	}

}
namespace Whisper.Messaging
{
	public partial class ListMessage
	{
		public List<Whisper.Chunks.TrippleID> List { get; set; }
	}

}
