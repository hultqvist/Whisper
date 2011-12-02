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
		public TreeChunk()
		{
			this.Directories = new List<TreeFile>();
			this.Files = new List<TreeFile>();
		}

		public static TrippleID GenerateChunk(string path, Repo repo)
		{
			string fullPath = Path.GetFullPath(path);
			TreeChunk tree = new TreeChunk();

			//Subdirectories
			string[] dirs = Directory.GetDirectories(fullPath);
			foreach (string d in dirs)
			{
				TreeFile df = new TreeFile();
				df.Name = Path.GetFileName(d);
				df.TreeChunkID = TreeChunk.GenerateChunk(d, repo);
				tree.Directories.Add(df);
			}

			//Files
			string[] files = Directory.GetFiles(fullPath);
			foreach (string f in files)
			{
				TreeFile ff = new TreeFile();
				ff.Name = Path.GetFileName(f);
				ff.TreeChunkID = StreamChunk.GenerateChunk(f, repo);
				tree.Files.Add(ff);
			}

			Chunk treeChunk = new Chunk(TreeChunk.SerializeToBytes(tree));
			ChunkHash ch = repo.WriteChunk(treeChunk);

			return new TrippleID(treeChunk.ClearHash, ch, treeChunk.CustomID);
		}

		public static void Extract(Repo store, TrippleID id, string targetPath)
		{
			Directory.CreateDirectory(targetPath);

			ChunkHash cid = id.ChunkHash;
			if (id.CustomID != null)
			{
				ChunkHash custom = store.GetCustomHash(id.CustomID);
				if (custom != null)
					cid = custom;
			}
			Chunk c = store.ReadChunk(cid);
			if (c.Verify(id) == false)
				throw new InvalidDataException("Invalid hash data");
			TreeChunk tree = TreeChunk.Deserialize(c.Data);
			;

			foreach (TreeFile file in tree.Files)
			{
				StreamChunk.Extract(store, file.TreeChunkID, Path.Combine(targetPath, file.Name));
			}
			
			foreach (TreeFile subdir in tree.Directories)
			{
				TreeChunk.Extract(store, subdir.TreeChunkID, Path.Combine(targetPath, subdir.Name));
			}
		}
	}
}

