using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Chunks;
using System.Text;

namespace Whisper.Chunks
{
	public class TreeChunk : BinaryChunk
	{
		public List<TreeFile> Directories = new List<TreeFile>();
		public List<TreeFile> Files = new List<TreeFile>();

		public static Chunk GenerateBlob(string path, Storage storage, ICollection<ChunkHash> blobList)
		{
			string fullPath = Path.GetFullPath(path);
			TreeChunk tree = new TreeChunk();
			
			//Subdirectories
			string[] dirs = Directory.GetDirectories(fullPath);
			foreach (string d in dirs)
			{
				TreeFile df = new TreeFile();
				df.Name = Path.GetFileName(d);
				df.Tripple = TreeChunk.GenerateBlob(d, storage, blobList).ClearID;
				tree.Directories.Add(df);
			}
			
			//Files
			string[] files = Directory.GetFiles(fullPath);
			foreach (string f in files)
			{
				TreeFile ff = new TreeFile();
				ff.Name = Path.GetFileName(f);
				ff.Tripple = StreamChunk.GenerateBlob(f, storage, blobList).ClearID;
				tree.Files.Add(ff);
			}
			
			Chunk treeBlob = tree.ToBlob();
			storage.WriteChunk(treeBlob);
			
			if (blobList != null)
				blobList.Add(treeBlob.ChunkHash);
			return treeBlob;
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
			TreeChunk tree = new TreeChunk();
			tree.ReadChunk(c);
			
			foreach (TreeFile file in tree.Files)
			{
				StreamChunk.Extract(store, file.Tripple, Path.Combine(targetPath, file.Name));
			}
			
			foreach (TreeFile subdir in tree.Directories)
			{
				TreeChunk.Extract(store, subdir.Tripple, Path.Combine(targetPath, subdir.Name));
			}
		}

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			writer.Write((int) Directories.Count);
			foreach (TreeFile d in Directories)
			{
				d.WriteChunk(writer);
			}
			
			writer.Write((int) Files.Count);
			foreach (TreeFile f in Files)
			{
				f.WriteChunk(writer);
			}
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			int dirs = reader.ReadInt32();
			for (int n = 0; n < dirs; n++)
			{
				Directories.Add(TreeFile.FromBlob(reader));
			}
			
			int files = reader.ReadInt32();
			for (int n = 0; n < files; n++)
			{
				Files.Add(TreeFile.FromBlob(reader));
			}
		}
		
		#endregion
		
	}

	public class TreeFile : BinaryChunk
	{
		public string Name { get; set; }

		/// <summary>
		/// StreamMessage blob for files.
		/// Tree for subdirectories.
		/// </summary>
		public TrippleID Tripple { get; set; }

		#region Blob Reader/Writer

		internal override void WriteChunk(BinaryWriter writer)
		{
			Tripple.WriteChunk(writer);
			WriteString(writer, Name);
		}

		internal override void ReadChunk(BinaryReader reader)
		{
			Tripple = TrippleID.FromBlob(reader);
			Name = ReadString(reader);
		}

		static internal TreeFile FromBlob(BinaryReader reader)
		{
			TreeFile file = new TreeFile();
			file.ReadChunk(reader);
			return file;
		}
		
		#endregion
	}
}

