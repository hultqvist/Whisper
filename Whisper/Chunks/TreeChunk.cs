using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Chunks;
using System.Text;
using ProtocolBuffers;

namespace Whisper.Chunks
{
	public partial class TreeChunk
	{
		public TreeChunk()
		{
			this.Directories = new List<TreeFile>();
			this.Files = new List<TreeFile>();
		}

		public static Chunk GenerateChunk(string path, Storage storage, ICollection<ChunkHash> chunkList)
		{
			string fullPath = Path.GetFullPath(path);
			TreeChunk tree = new TreeChunk();
			
			//Subdirectories
			string[] dirs = Directory.GetDirectories(fullPath);
			foreach (string d in dirs)
			{
				TreeFile df = new TreeFile();
				df.Name = Path.GetFileName(d);
				df.TreeChunkID = TreeChunk.GenerateChunk(d, storage, chunkList).TrippleID;
				tree.Directories.Add(df);
			}
			
			//Files
			string[] files = Directory.GetFiles(fullPath);
			foreach (string f in files)
			{
				TreeFile ff = new TreeFile();
				ff.Name = Path.GetFileName(f);
				ff.TreeChunkID = StreamChunk.GenerateChunk(f, storage, chunkList).TrippleID;
				tree.Files.Add(ff);
			}

			Chunk treeChunk = new Chunk(TreeChunk.SerializeToBytes(tree));
			storage.WriteChunk(treeChunk);
			
			if (chunkList != null)
				chunkList.Add(treeChunk.DataHash);
			return treeChunk;
		}

		public static void Extract(Storage store, TrippleID id, string targetPath)
		{
			Directory.CreateDirectory(targetPath);
			
			ChunkHash cid = store.GetCustomHash(id.CustomID);
			if (cid == null)
				cid = id.ChunkHash;
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
