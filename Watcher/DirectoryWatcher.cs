using System;
using Whisper;
using Whisper.Encryption;
using System.IO;

namespace Whisper.Watcher
{
	/// <summary>
	/// Watch for changes in a directory and send them to the remote repo
	/// </summary>
	public class DirectoryWatcher
	{
		/// <summary>
		/// Path to directory being watched
		/// </summary>
		readonly string path;
		/// <summary>
		/// Where to send changes
		/// </summary>
		readonly Repo remote;
		/// <summary>
		/// To whom we address the message
		/// </summary>
		readonly PublicKey recipient;

		public DirectoryWatcher(string directory, PublicKey recipient, Repo remoteRepo)
		{
			this.path = Path.GetFullPath(directory);
			if(Directory.Exists(this.path) == false)
				throw new ArgumentException("Directory does not exist");
			this.recipient = recipient;
			this.remote = remoteRepo;
		}
	}
}

