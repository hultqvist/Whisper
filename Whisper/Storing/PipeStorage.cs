using System;
using System.IO;
using System.Diagnostics;
using Whisper.Chunks;
using System.Collections.Generic;

namespace Whisper.Storing
{
	/// <summary>
	/// Store into a pipe to another program.
	/// The other end of the pipe is implemented in project WhisperServer file Server.cs
	/// </summary>
	public class PipeStorage : Storage
	{
		BinaryReader reader;
		BinaryWriter writer;

		public PipeStorage(string command, string arguments)
		{
			Process p = new Process();
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			p.Start();

			reader = new BinaryReader(p.StandardOutput.BaseStream);
			writer = new BinaryWriter(p.StandardInput.BaseStream);
		}

		private enum PipeProtocol
		{
			GetCustomHash,
			ReadChunk,
			WriteChunk,
			GetMessageList,
			StoreMessage
		}

		public override ChunkHash GetCustomHash(CustomID customID)
		{
			writer.Write((int) PipeProtocol.GetCustomHash);
			customID.WriteChunk(writer);

			int reply = reader.ReadInt32();

			if (reply == 0)
				return null;
			if (reply == 1)
				return new ChunkHash(new Hash(reader.ReadBytes(32)));

			throw new InvalidOperationException("Invalid reply");
		}

		public override Chunk ReadChunk(ChunkHash chunkHash)
		{
			//Read Chunk Data
			writer.Write((int) PipeProtocol.ReadChunk);
			chunkHash.WriteChunk(writer);

			int size = reader.ReadInt32();
			Chunk c = new Chunk();
			c.Data = reader.ReadBytes(size);
			c.ChunkHash = new ChunkHash(Hash.ComputeHash(c.Data));
			//Verify Hash
			if (c.ChunkHash.Equals(chunkHash) == false)
				throw new InvalidDataException("Hash mismatch: " + chunkHash);

			//Read Chunk Keys
			ChunkKeys ck = new ChunkKeys();
			ck.ReadChunk(reader);
			if (ck.Keys.Count > 0)
				c.Keys = ck;

			return c;
		}

		public override void WriteChunk(Chunk chunk)
		{
			writer.Write((int) PipeProtocol.WriteChunk);
			writer.Write((int) chunk.Data.Length);
			writer.Write(chunk.Data);

			int status = reader.ReadInt32();
			if (status != 0)
				throw new InvalidOperationException();
		}

		public override ICollection<ChunkHash> GetMessageList()
		{
			writer.Write((int) PipeProtocol.GetMessageList);

			int count = reader.ReadInt32();
			List<ChunkHash> list = new List<ChunkHash>();
			for (int n = 0; n < count; n++)
			{
				ChunkHash ch = new ChunkHash(Hash.FromChunk(reader));
				list.Add(ch);
			}
			return list;
		}

		public override void StoreMessage(ChunkHash chunkHash)
		{
			writer.Write((int) PipeProtocol.StoreMessage);
			chunkHash.WriteChunk(writer);

			int status = reader.ReadInt32();
			if (status != 0)
				throw new InvalidOperationException();
		}

	}
}

