using System;
using Whisper;
using Whisper.Encryption;
using System.IO;
using System.Collections.Generic;
using System.Threading;

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
		/// Prefix for messages
		/// </summary>
		readonly string prefix;

		public DirectoryWatcher (string directory, Repo remoteRepo, string prefix)
		{
			this.path = Path.GetFullPath (directory);
			this.remote = remoteRepo;
			this.prefix = prefix;
		}

		public void Run ()
		{
			//Load previous state
			//TODO

			//Scan directory
			Console.WriteLine ("Scanning...");
			string[] files = Directory.GetFiles (path, "*", SearchOption.AllDirectories);
			foreach (string f in files)
				Console.WriteLine (f);

			//Start watching
			using (FileSystemWatcher fsw = new FileSystemWatcher ()) {
				fsw.Path = path;
				fsw.Changed += FileChanged;
				fsw.Deleted += FileDeleted;
				//fsw.Created += FileChanged;
				fsw.Renamed += FileRenamed;
				fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
				fsw.IncludeSubdirectories = true;
				fsw.InternalBufferSize = 4096 * 10;
				fsw.EnableRaisingEvents = true;

				while (true) {
					//var res = fsw.WaitForChanged (WatcherChangeTypes.All);
					Thread.Sleep (5000);
				}
			}

		}

		void FileChanged (object sender, FileSystemEventArgs e)
		{
			Console.WriteLine ("Modified: " + e.FullPath);

			//TODO: Keep a paralell structure with
			// * File properties: size, lastWriteUtc - to know if a file was modified at startup
			// * TreeChunk equivalent.
			// Will probably need lots of remade code from TreeChunk
		}

		void FileRenamed (object sender, RenamedEventArgs e)
		{
			Console.WriteLine (e.OldFullPath + " -> " + e.FullPath);
		}

		void FileDeleted (object sender, FileSystemEventArgs e)
		{
			Console.WriteLine ("Removed: " + e.FullPath);
		}
	}
}

