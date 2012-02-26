using System;
using System.Collections.Generic;
using Whisper.Chunks;
using Whisper.Messages;

namespace Whisper.Repos
{
	/// <summary>
	/// Record all chunks stored to be used in a RouteMessage
	/// </summary>
	public class RouteRepo : RepoFilter
	{
		public RouteMessage RouteMessage { get; private set; }

		public RouteRepo (Repo backendRepo) : base(backendRepo)
		{
			this.RouteMessage = new RouteMessage ();
		}

		public override ChunkHash WriteChunk (Chunk chunk)
		{
			ChunkHash hash = base.WriteChunk (chunk);
			this.RouteMessage.Chunks.Add (hash.bytes);
			return hash;
		}
	}
}

