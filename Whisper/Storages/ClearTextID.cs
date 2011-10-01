using System;
using Whisper.Chunks;
namespace Whisper.Storages
{
	/// <summary>
	/// Generates CustomID that equals the cleartext hash
	/// </summary>
	public class ClearTextID : IGenerateID
	{
		public CustomID GetID(Chunk chunk)
		{
			return CustomID.FromBytes(chunk.ClearHash.bytes);
		}
	}
}

