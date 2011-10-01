using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Whisper;
using ProtocolBuffers;
using System.Text;

namespace Whisper.Keys
{
	public class DiskKeyStorage : KeyStorage
	{
		private const string privateSuffix = ".private";
		private const string publicSuffix = ".public";
		readonly string path;

		/// <summary>
		/// Default to ~/.config/Whisper/Keys/ on linux
		/// </summary>
		public DiskKeyStorage ()
		{
			path = Path.Combine (Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData),
				"Whisper"), "Keys");
			Directory.CreateDirectory (path);
		}

		public DiskKeyStorage (string path)
		{
			this.path = path;
			Directory.CreateDirectory (path);
		}

		public override PrivateKey DefaultKey {
			get { return GetPrivate ("Default"); }
			set { throw new NotImplementedException (); }
		}

		private IEnumerable<PublicKey> publicKeys = null;
		private IEnumerable<PrivateKey> privateKeys = null;

		public override IEnumerable<PublicKey> PublicKeys {
			get {
				if (publicKeys == null)
					publicKeys = GetPublicKeys ();
				return publicKeys;
			}
		}

		public override IEnumerable<PrivateKey> PrivateKeys {
			get {
				if (privateKeys == null)
					privateKeys = GetPrivateKeys ();
				return privateKeys;
			}
		}

		public override void Add (string name, IKey key)
		{
			string keyPath = Path.Combine (this.path, name);

			if (File.Exists (keyPath + privateSuffix) || File.Exists (keyPath + publicSuffix))
				throw new ArgumentException ("Key with that name already exists");

			WriteKey (key, name);
		}

		public override PublicKey GetPublic (string name)
		{
			string keyPath = Path.Combine (this.path, name + publicSuffix);
			PublicKey key = new PublicKey (File.ReadAllText (keyPath, Encoding.UTF8));
			key.Name = name;
			return key;
		}

		public override PrivateKey GetPrivate (string name)
		{
			string keyPath = Path.Combine (this.path, name + privateSuffix);
			PrivateKey key = new PrivateKey (File.ReadAllText (keyPath, Encoding.UTF8));
			key.Name = name;
			return key;
		}

		private IEnumerable<PublicKey> GetPublicKeys ()
		{
			string[] files = Directory.GetFiles (this.path, "*" + publicSuffix);
			List<PublicKey > keys = new List<PublicKey> ();
			foreach (string name in files) {
				PublicKey key = GetPublic (Path.GetFileNameWithoutExtension (name));
				keys.Add (key);
			}

			return keys;
		}

		private IEnumerable<PrivateKey> GetPrivateKeys ()
		{
			string[] files = Directory.GetFiles (this.path, "*" + privateSuffix);
			List<PrivateKey > keys = new List<PrivateKey> ();
			foreach (string name in files) {
				PrivateKey key = GetPrivate (Path.GetFileNameWithoutExtension (name));
				keys.Add (key);
			}

			return keys;
		}

		public void WriteKey (IKey key, string name)
		{
			string keyPath = Path.Combine (this.path, name);

			//Saving private part of key
			if (key is PrivateKey) {
				PrivateKey priv = key as PrivateKey;
				
				File.WriteAllText (keyPath + privateSuffix, priv.ToXml (), Encoding.UTF8);

				File.WriteAllText (keyPath + publicSuffix, priv.PublicKey.ToXml (), Encoding.UTF8);
			} else {
				//Saving Public part of the key
				File.WriteAllText (keyPath + publicSuffix, key.ToXml (), Encoding.UTF8);
			}

		}
	}
}

