using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg;
using System.IO;

namespace Whisper.Keys
{
	/// <summary>
	/// This is a test code gathered from the BouncyCastle examples.
	/// It may in the future be used to encrypt chunks.
	/// </summary>
	public class Pgp
	{
		const string password = "123456-7";

		public Pgp()
		{
			PgpSecretKey key = GenerateKey();
			SaveKey(key, "pubkey.pgp", "seckey.pgp");

			PgpPublicKey pubKey = LoadPubKey("pubkey.pgp");
			PgpSecretKey secKey = LoadSecKey("seckey.pgp");

			byte[] clearText = GenerateTestData();
			byte[] cipherText = Encrypt(clearText, pubKey);
			byte[] decryptText = Decrypt(cipherText, secKey);

			for (int n = 0; n < clearText.Length; n++)
				if (clearText[n] != decryptText[n])
					throw new InvalidDataException();

			throw new Exception("PGP test done");
		}

		public PgpSecretKey GenerateKey()
		{
			IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");
			kpg.Init(new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 25));

			AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

			PgpSecretKey secretKey = new PgpSecretKey(
                PgpSignature.DefaultCertification,
                PublicKeyAlgorithmTag.RsaGeneral,
                kp.Public, kp.Private,
                DateTime.UtcNow,
                "test@silentorbit.com",
                SymmetricKeyAlgorithmTag.Cast5,
                password.ToCharArray(), null, null, new SecureRandom());

			return secretKey;
		}

		public void SaveKey(PgpSecretKey key, string publicPath, string secretPath)
		{
			using (FileStream pubStream = new FileStream(publicPath, FileMode.Create))
				key.PublicKey.Encode(pubStream);
			using (FileStream secStream = new FileStream(secretPath, FileMode.Create))
				key.Encode(secStream);
		}

		public PgpPublicKey LoadPubKey(string path)
		{
			using (FileStream keyStream = File.OpenRead(path))
			{
				Stream decStream = PgpUtilities.GetDecoderStream(keyStream);
				PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(decStream);

				foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
				{
					foreach (PgpPublicKey key in keyRing.GetPublicKeys())
					{
						if (key.IsEncryptionKey)
						{
							return key;
						}
					}
				}
				throw new ArgumentException("Key not found");
			}
		}

		public PgpSecretKey LoadSecKey(string path)
		{
			using (Stream keyIn = File.OpenRead(path))
			{
				Stream decStream = PgpUtilities.GetDecoderStream(keyIn);
				PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(decStream);
				foreach (PgpSecretKeyRing keyRing in pgpSec.GetKeyRings())
				{
					foreach (PgpSecretKey key in keyRing.GetSecretKeys())
					{
						if (key.IsSigningKey)
						{
							return key;
						}
					}
				}
				throw new ArgumentException("Can't find signing key in key ring.");
			}
		}

		public byte[] GenerateTestData()
		{
			Random r = new Random();
			byte[] test = new byte[50];
			r.NextBytes(test);
			return test;
		}

		public byte[] Encrypt(byte[] clearText, PgpPublicKey pubKey)
		{
			byte[] bytes = Compress(clearText);

			PgpEncryptedDataGenerator encGen = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, true, new SecureRandom());
			encGen.AddMethod(pubKey);

			MemoryStream outputStream = new MemoryStream();
			Stream cOut = encGen.Open(outputStream, bytes.Length);

			cOut.Write(bytes, 0, bytes.Length);
			cOut.Close();
			return outputStream.ToArray();
		}

		public byte[] Decrypt(byte[] cipherText, PgpSecretKey secKey)
		{
			MemoryStream ms = new MemoryStream(cipherText);
			Stream inputStream = PgpUtilities.GetDecoderStream(ms);

			PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
			PgpEncryptedDataList enc;

			PgpObject o = pgpF.NextPgpObject();
			// the first object might be a PGP marker packet.
			if (o is PgpEncryptedDataList)
				enc = (PgpEncryptedDataList) o;
			else
				enc = (PgpEncryptedDataList) pgpF.NextPgpObject();

			PgpPublicKeyEncryptedData pbe = null;

			foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
			{
				pbe = pked;
				break;
			}

			Stream clear = pbe.GetDataStream(secKey.ExtractPrivateKey(password.ToCharArray()));

			PgpObjectFactory plainFact = new PgpObjectFactory(clear);

			PgpObject message = plainFact.NextPgpObject();

			if (message is PgpCompressedData)
			{
				PgpCompressedData cData = (PgpCompressedData) message;
				PgpObjectFactory pgpFact = new PgpObjectFactory(cData.GetDataStream());

				message = pgpFact.NextPgpObject();
			}

			MemoryStream clearStream = new MemoryStream();

			if (message is PgpLiteralData)
			{
				PgpLiteralData ld = (PgpLiteralData) message;

				Stream unc = ld.GetInputStream();
				byte[] buffer = new byte[4096];
				int read;
				while ((read = unc.Read(buffer, 0, buffer.Length)) > 0)
					clearStream.Write(buffer, 0, read);
			}
			else if (message is PgpOnePassSignatureList)
				throw new PgpException("encrypted message contains a signed message - not literal data.");
			else
				throw new PgpException("message is not a simple encrypted file - type unknown.");

			if (pbe.IsIntegrityProtected())
			{
				if (!pbe.Verify())
				{
					throw new InvalidDataException("message failed integrity check");
				}
				else
				{
					Console.Error.WriteLine("message integrity check passed");
				}
			}
			else
			{
				Console.Error.WriteLine("no message integrity check");
			}
			return clearStream.ToArray();
		}

		private static byte[] Compress(byte[] clearData)
		{
			MemoryStream bOut = new MemoryStream();

			PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
			Stream cos = comData.Open(bOut); // open it with the final destination
			PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();

			// we want to Generate compressed data. This might be a user option later,
			// in which case we would pass in bOut.
			Stream pOut = lData.Open(
				cos, // the compressed output stream
                PgpLiteralData.Binary,
                "", // "filename" to store
                clearData.Length,
                DateTime.UtcNow
            );

			pOut.Write(clearData, 0, clearData.Length);
			pOut.Close();

			comData.Close();

			return bOut.ToArray();
		}
	}
}

