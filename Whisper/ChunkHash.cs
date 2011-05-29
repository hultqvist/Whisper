using System;
using System.IO;
namespace Whisper
{
	/// <summary>
	/// A hash of the blob data in transit
	/// </summary>
	public class ChunkHash : Hash
	{
		public ChunkHash(Hash hash) : base(hash)
		{
		}

		public static new ChunkHash FromString(string id)
		{
			return new ChunkHash(Hash.FromString(id));
		}

		internal static new ChunkHash FromChunk(BinaryReader reader)
		{
			return new ChunkHash(Hash.FromChunk(reader));
		}

	}
}
