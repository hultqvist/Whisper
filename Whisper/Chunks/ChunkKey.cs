using System;
namespace Whisper.Chunks
{
	/// <summary>
	/// Encrypted key used to decrypt a blob
	/// </summary>
	public class ChunkKey
	{
		/// <summary>
		/// Encrypted AES key
		/// </summary>
		public byte[] bytes;
	}
}

