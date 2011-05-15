using System;
using System.IO;
using System.Collections.Generic;
using Whisper.Storing;
using Whisper.Blobing;
using System.Text;

namespace Whisper.Blobing
{
	public class TreeBlob : BinaryBlob
	{
		public List<TreeFile> Directories = new List<TreeFile>();
		public List<TreeFile> Files = new List<TreeFile>();

		public static Blob GenerateBlob(string path, Storage storage, ICollection<BlobHash> blobList)
		{
			string fullPath = Path.GetFullPath(path);
			TreeBlob tree = new TreeBlob();
			
			//Subdirectories
			string[] dirs = Directory.GetDirectories(fullPath);
			foreach (string d in dirs)
			{
				TreeFile df = new TreeFile();
				df.Name = Path.GetFileName(d);
				df.Tripple = TreeBlob.GenerateBlob(d, storage, blobList).ClearID;
				tree.Directories.Add(df);
			}
			
			//Files
			string[] files = Directory.GetFiles(fullPath);
			foreach (string f in files)
			{
				TreeFile ff = new TreeFile();
				ff.Name = Path.GetFileName(f);
				ff.Tripple = StreamBlob.GenerateBlob(f, storage, blobList).ClearID;
				tree.Files.Add(ff);
			}
			
			Blob treeBlob = tree.ToBlob();
			storage.WriteBlob(treeBlob);
			
			if (blobList != null)
				blobList.Add(treeBlob.BlobHash);
			return treeBlob;
		}

		public static void Extract(Storage store, TrippleID id, string targetPath)
		{
			Directory.CreateDirectory(targetPath);
			
			BlobHash cid = store.GetBlobHash(id.CustomID);
			if (cid == null)
				cid = id.BlobHash;
			Blob c = store.ReadBlob(cid);
			if (c.Verify(id) == false)
				throw new InvalidDataException("Invalid hash data");
			TreeBlob tree = new TreeBlob();
			tree.ReadBlob(c);
			
			foreach (TreeFile file in tree.Files)
			{
				StreamBlob.Extract(store, file.Tripple, Path.Combine(targetPath, file.Name));
			}
			
			foreach (TreeFile subdir in tree.Directories)
			{
				TreeBlob.Extract(store, subdir.Tripple, Path.Combine(targetPath, subdir.Name));
			}
		}

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			writer.Write((int) Directories.Count);
			foreach (TreeFile d in Directories)
			{
				d.WriteBlob(writer);
			}
			
			writer.Write((int) Files.Count);
			foreach (TreeFile f in Files)
			{
				f.WriteBlob(writer);
			}
		}

		internal override void ReadBlob(BinaryReader reader)
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

	public class TreeFile : BinaryBlob
	{
		public string Name { get; set; }

		/// <summary>
		/// StreamMessage blob for files.
		/// Tree for subdirectories.
		/// </summary>
		public TrippleID Tripple { get; set; }

		#region Blob Reader/Writer

		internal override void WriteBlob(BinaryWriter writer)
		{
			Tripple.WriteBlob(writer);
			WriteString(writer, Name);
		}

		internal override void ReadBlob(BinaryReader reader)
		{
			Tripple = TrippleID.FromBlob(reader);
			Name = ReadString(reader);
		}

		static internal TreeFile FromBlob(BinaryReader reader)
		{
			TreeFile file = new TreeFile();
			file.ReadBlob(reader);
			return file;
		}
		
		#endregion
	}
}

