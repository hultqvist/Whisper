using System;
using System.IO;
using System.Text;
namespace Whisper.Blobs
{
	/// <summary>
	/// This base class is used by classes that need to be stored in binary format.
	/// </summary>
	public abstract class BinaryBlob
	{
		/// <summary>
		/// Read data from blob into class structure
		/// </summary>
		internal abstract void ReadBlob(BinaryReader reader);

		/// <summary>
		/// Write class structure into a blob
		/// </summary>
		internal abstract void WriteBlob(BinaryWriter writer);

		/// <summary>
		/// Helper to store variable length strings in binary format.
		/// </summary>
		protected void WriteString(BinaryWriter writer, string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			writer.Write((int) bytes.Length);
			writer.Write(bytes);
		}

		/// <summary>
		/// Helper to read strings from a binary format
		/// </summary>
		protected string ReadString(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			byte[] bytes = reader.ReadBytes(length);
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary>
		/// Helper to store variable length strings in binary format.
		/// </summary>
		protected void WriteVarBytes(BinaryWriter writer, byte[] data)
		{
			writer.Write((int) data.Length);
			writer.Write(data);
		}

		/// <summary>
		/// Helper to read strings from a binary format
		/// </summary>
		protected byte[] ReadVarBytes(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			return reader.ReadBytes(length);
		}

		/// <summary>
		/// Read blob data into class
		/// </summary>
		internal void ReadBlob(Blob blob)
		{
			MemoryStream ms = new MemoryStream(blob.Data);
			BinaryReader br = new BinaryReader(ms);
			ReadBlob(br);
		}

		/// <summary>
		/// Generate a blob from class.
		/// The blob must be stored afterwards.
		/// </summary>
		internal Blob ToBlob()
		{
			MemoryStream ms = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(ms);
			WriteBlob(bw);
			return new Blob(ms.ToArray());
		}
	}
}

