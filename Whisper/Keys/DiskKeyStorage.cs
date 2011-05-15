using System;
using Whisper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Whisper.Keys
{
	public class DiskKeyStorage : KeyStorage
	{
		readonly string path;

		public DiskKeyStorage(string path)
		{
			this.path = path;
		}

		public override PrivateKey DefaultKey {
			get { return FromName("Default") as PrivateKey; }
			set {
				throw new NotImplementedException();
			}
		}

		public override PublicKey FromName(string name)
		{
			//Determine if we have a private or public key
			string keyPath = Path.Combine(this.path, name);
			if (File.Exists(keyPath + ".privatekey"))
			{
				PrivateKey pk = new PrivateKey();
				ReadKey(pk, name + ".publickey");
				return pk;
			}
			if (File.Exists(keyPath + ".publickey"))
			{
				PublicKey pk = new PublicKey();
				ReadKey(pk, name + ".privatekey");
				return pk;
			}
			return null;
		}

		private IEnumerable<PublicKey> publicKeys = new List<PublicKey>();
		private IEnumerable<PrivateKey> privateKeys = new List<PrivateKey>();

		public override IEnumerable<PublicKey> PublicKeys {
			get {
				if (publicKeys == null)
					publicKeys = GetPublicKeys();
				return publicKeys;
			}
		}
		public override IEnumerable<PrivateKey> PrivateKeys {
			get {
				if (privateKeys == null)
					privateKeys = GetPrivateKeys();
				return privateKeys;
			}
		}

		private IEnumerable<PublicKey> GetPublicKeys()
		{
			string[] files = Directory.GetFiles(this.path, "*.publickey");
			List<PublicKey> keys = new List<PublicKey>();
			foreach (string name in files)
			{
				PublicKey key = new PublicKey();
				ReadKey(key, name);
				keys.Add(key);
			}

			return keys;
		}

		private IEnumerable<PrivateKey> GetPrivateKeys()
		{
			string[] files = Directory.GetFiles(this.path, "*.privatekey");
			List<PrivateKey> keys = new List<PrivateKey>();
			foreach (string name in files)
			{
				PrivateKey key = new PrivateKey();
				ReadKey(key, name);
				keys.Add(key);
			}

			return keys;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="key">
		/// A <see cref="PublicKey"/> or a <see cref="PrivateKey"/>
		/// </param>
		/// <param name="filename">
		/// Filename to key file.
		/// </param>
		private void ReadKey(PublicKey key, string filename)
		{
			string keyPath = Path.Combine(this.path, filename);
			using (BinaryReader br = new BinaryReader(new FileStream(keyPath, FileMode.Open)))
			{
				key.ReadBlob(br);
			}
		}
	}
}

