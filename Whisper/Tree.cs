using System;
using System.Collections;
using System.Collections.Generic;
using Whisper.Chunks;
using Whisper.Messages;
using Whisper.Storages;
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

		private ICollection<Storage> _storage = null;

		public ICollection<Storage> Storage {
			get {
				if (_storage == null)
					_storage = new List<Storage> ();
				return _storage;
			}
			set { _storage = value; }
		}

		#endregion

		#region Output Data
		public ICollection<ChunkHash> ChunkList { get; private set; }

		public Chunk tree { get; set; }
		#endregion

		/// <summary>
		/// Final top storage generated in GenerateTreeChunk from EncryptKeys and Storage
		/// </summary>
		private Storage storage;

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

			if (Storage.Count == 0)
				throw new ArgumentException ("this.Storage must contain at least one storage");

			#endregion

			//Prepare Storage
			Storage s;
			if (this.Storage.Count == 1)
				s = this.Storage.First ();
			else
				s = new MultiStorage (this.Storage);

			//Prepare encryption
			if (EncryptKeys.Count > 0) {
				IGenerateID id = null;
				if (EncryptKeys.Count == 1)
					id = new RecipientID (EncryptKeys.First ());
				s = new EncryptedStorage (s, null, id);
				EncryptedStorage es = s as EncryptedStorage;
				foreach (PublicKey key in EncryptKeys)
					es.AddKey (key);
			}

			//Storage preparation done
			this.storage = s;

			this.tree = TreeChunk.GenerateChunk (this.SourcePath, this.storage, this.ChunkList);

			//TreeMessage
			TreeMessage tm = new TreeMessage (this.tree, this.TargetName);
			Chunk smb = Message.ToChunk (tm, this.SigningKey);
			this.storage.WriteChunk (smb);
			this.ChunkList.Add (smb.ChunkHash);
			return smb.ChunkHash;
		}

	}
}

