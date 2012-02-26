using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Repos;
using Whisper.Chunks;
using System.Text;
using ProtocolBuffers;
using Whisper.Messages;

namespace Whisper.ChunkGenerator
{
	public partial class TreeChunk
	{
		public TreeChunk ()
		{
			this.Directories = new List<TreeFile> ();
			this.Files = new List<TreeFile> ();
		}

		public static ChunkHash GenerateChunk (string path, Repo repo)
		{
			string fullPath = Path.GetFullPath (path);
			TreeChunk tree = new TreeChunk ();

			//Subdirectories
			string[] dirs = Directory.GetDirectories (fullPath);
			foreach (string d in dirs) {
				TreeFile df = new TreeFile ();
				df.Name = Path.GetFileName (d);
				df.TreeChunkHash = TreeChunk.GenerateChunk (d, repo).bytes;
				tree.Directories.Add (df);
			}

			//Files
			string[] files = Directory.GetFiles (fullPath);
			foreach (string f in files) {
				TreeFile ff = new TreeFile ();
				ff.Name = Path.GetFileName (f);
				ff.TreeChunkHash = StreamChunk.GenerateChunk (f, repo).bytes;
				tree.Files.Add (ff);
			}

			Chunk treeChunk = new Chunk (TreeChunk.SerializeToBytes (tree));
			ChunkHash ch = repo.WriteChunk (treeChunk);

			return ch;
		}

		public static void Extract (Repo store, ChunkHash cid, string targetPath)
		{
			Directory.CreateDirectory (targetPath);

			Chunk c = store.ReadChunk (cid);
			TreeChunk tree = TreeChunk.Deserialize (c.Data);

			foreach (TreeFile file in tree.Files) {
				StreamChunk.Extract (store, ChunkHash.FromHashBytes (file.TreeChunkHash), Path.Combine (targetPath, file.Name));
			}

			foreach (TreeFile subdir in tree.Directories) {
				TreeChunk.Extract (store, ChunkHash.FromHashBytes (subdir.TreeChunkHash), Path.Combine (targetPath, subdir.Name));
			}
		}
	}
}

