using System;
using System.IO;

namespace Whisper.Watcher
{
	/// <summary>
	/// The current state of a single file
	/// </summary>
	public class WatchState
	{
		/// <summary>
		/// Last Write Time in UTC
		/// </summary>
		public DateTime LastWrite { get; set; }

		public long FileSize { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Whisper.Watcher.WatchState"/> class.
		/// </summary>
		/// <param name='path'>
		/// Full path to file
		/// </param>
		public WatchState (string path)
		{
			FileInfo fi = new FileInfo (path);
			LastWrite = fi.LastWriteTimeUtc;
			FileSize = fi.Length;
		}
	}
}

