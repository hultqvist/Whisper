using System;
using System.Collections;
using System.Collections.Generic;
using Whisper.Chunks;
using Whisper.Messages;
using Whisper.Repos;
using Whisper.Keys;

namespace Whisper
{
	public class Tree
	{
		public Tree ()
		{
			this.ChunkList = new List<ChunkHash> ();
		}

		#region Input Parameters
		//Source
		public string SourcePath { get; set; }
		//Target
		public string TargetName { get; set; }
		/// <summary>
		/// Set to sign the TreeMessage
		/// </summary>
		public PrivateKey SigningKey { get; set; }

		private ICollection<PublicKey> _encryptKeys = null;

		public ICollection<PublicKey> EncryptKeys {
			get {
				if (_encryptKeys == null)
					_encryptKeys = new List<PublicKey> ();
				return _encryptKeys;
			}
			set { _encryptKeys = value; }
		}

		private ICollection<Repo> _repo = null;

		public ICollection<Repo> Repo {
			get {
				if (_repo == null)
					_repo = new List<Repo> ();
				return _repo;
			}
			set { _repo = value; }
		}

		#endregion

		#region Output Data
		public ICollection<ChunkHash> ChunkList { get; private set; }

		public Chunk tree { get; set; }
		#endregion

		/// <summary>
		/// Final top repo generated in GenerateTreeChunk by nesting EncryptKeys and Storage
		/// </summary>
		private Repo repo;

		/// <summary>
		/// Generate, encrypt and store the tree.
		/// Generates a treemessage and store it.
		/// Return the ChunkHash of the final tree message.
		/// </summary>
		/// <returns>
		/// The <see cref="ChunkHash"/> of the TreeMessage
		/// </returns>
		public ChunkHash Generate ()
		{
			#region Check Parameters

			if (Repo.Count == 0)
				throw new ArgumentException ("this.Storage must contain at least one storage");

			#endregion

			//Prepare Storage
			Repo s;
			if (this.Repo.Count == 1)
				s = this.Repo.First ();
			else
				s = new MultiRepo (this.Repo);

			//Prepare encryption
			if (EncryptKeys.Count > 0) {
				IGenerateID id = null;
				if (EncryptKeys.Count == 1)
					id = new RecipientID (EncryptKeys.First ());
				s = new EncryptedRepo (s, null, id);
				EncryptedRepo es = s as EncryptedRepo;
				foreach (PublicKey key in EncryptKeys)
					es.AddKey (key);
			}

			//Storage preparation done
			this.repo = s;

			this.tree = TreeChunk.GenerateChunk (this.SourcePath, this.repo, this.ChunkList);

			//TreeMessage
			TreeMessage tm = new TreeMessage (this.tree, this.TargetName);
			Chunk smb = Message.ToChunk (tm, this.SigningKey);
			this.repo.WriteChunk (smb);
			this.ChunkList.Add (smb.ChunkHash);
			return smb.ChunkHash;
		}

	}
}

