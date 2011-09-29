using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Whisper;
using ProtocolBuffers;

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
		public DiskKeyStorage()
		{
			path = Path.Combine(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"Whisper"), "Keys");
			Directory.CreateDirectory(path);
		}

		public DiskKeyStorage(string path)
		{
			this.path = path;
			Directory.CreateDirectory(path);
		}

		public override PrivateKey DefaultKey {
			get { return GetPrivate("Default"); }
			set { throw new NotImplementedException(); }
		}

		private IEnumerable<PublicKey> publicKeys = null;
		private IEnumerable<PrivateKey> privateKeys = null;

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

		public override void Add(string name, IKey key)
		{
			string keyPath = Path.Combine(this.path, name);

			if (File.Exists(keyPath + privateSuffix) || File.Exists(keyPath + publicSuffix))
				throw new ArgumentException("Key with that name already exists");

			WriteKey(key, name);
		}

		public override PublicKey GetPublic(string name)
		{
			//Determine if we have a private or public key
			string keyPath = Path.Combine(this.path, name);
			if (File.Exists(keyPath + publicSuffix))
			{
				return ReadPublicKey(name);
			}
			return null;
		}

		public override PrivateKey GetPrivate(string name)
		{
			//Determine if we have a private or public key
			string keyPath = Path.Combine(this.path, name);
			if (File.Exists(keyPath + privateSuffix))
			{
				return ReadPrivateKey(name);
			}
			return null;
		}

		private IEnumerable<PublicKey> GetPublicKeys()
		{
			string[] files = Directory.GetFiles(this.path, "*" + publicSuffix);
			List<PublicKey > keys = new List<PublicKey>();
			foreach (string name in files)
			{
				PublicKey key = ReadPublicKey(Path.GetFileNameWithoutExtension(name));
				keys.Add(key);
			}

			return keys;
		}

		private IEnumerable<PrivateKey> GetPrivateKeys()
		{
			string[] files = Directory.GetFiles(this.path, "*" + privateSuffix);
			List<PrivateKey > keys = new List<PrivateKey>();
			foreach (string name in files)
			{
				PrivateKey key = ReadPrivateKey(Path.GetFileNameWithoutExtension(name));
				keys.Add(key);
			}

			return keys;
		}

		private PublicKey ReadPublicKey(string name)
		{
			string keyPath = Path.Combine(this.path, name + publicSuffix);
			using (Stream s = new FileStream(keyPath, FileMode.Open))
			{
				NamedPublicKey key = PublicKey.Deserialize<NamedPublicKey>(s);
				key.Name = name;
				return key;
			}
		}

		private PrivateKey ReadPrivateKey(string name)
		{
			string keyPath = Path.Combine(this.path, name + privateSuffix);
			using (Stream s = new FileStream(keyPath, FileMode.Open))
			{
				NamedPrivateKey key = new NamedPrivateKey();
				key.Name = name;
				Serializer.Read(s, key);
				return key;
			}
		}

		public void WriteKey(IKey key, string name)
		{
			string keyPath = Path.Combine(this.path, name);

			//Saving private part of key
			if (key is PrivateKey)
			{
				PrivateKey priv = key as PrivateKey;

				using (FileStream s = new FileStream(keyPath + privateSuffix, FileMode.CreateNew))
					Serializer.Write(s, priv);

				//This is a minor workaround until the protobuf-net library serialize the object as publickey only, ignoring private parameters.
				//Saving Public part of the key
				using (FileStream s = new FileStream(keyPath + publicSuffix, FileMode.CreateNew))
					Serializer.Write(s, priv.PublicKey);
			}
			else
			{
				//Saving Public part of the key
				using (FileStream s = new FileStream(keyPath + publicSuffix, FileMode.CreateNew))
					Serializer.Write(s, (PublicKey) key);
			}

		}
	}
}

