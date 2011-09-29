using System;
using System.IO;
using Whisper.Keys;
using Whisper.Storing;

namespace Test
{
	public class Test1
	{
		public static void Run()
		{
			//Start with clean setup
			Cleanup();

			if (Directory.Exists("Source") == false)
			{
				Console.WriteLine(@"Create a directory named ""Source"", fill it with some test files and run this command again");
				return;
			}

			PrivateKey senderKey = new PrivateKey("<RSAKeyValue><Modulus>mAQzrHhdry4jiJwDeDWU+CYKZQPtCljXptp8tFEqAfdDPtk8oBOW4VGbvwuLLaeTWogFt5unRR1WogYby0Qg2+seFWARNvCnP1w7/MB5ctAmeeILIuH7FU45k6eTvfZC3WTrkfYC+jS3oDuS4PG1386Ex84dkQoSgPt98sy0Ml0=</Modulus><Exponent>EQ==</Exponent><P>qg8gycxbDoquKHkCb/o9MyzQ/9wOWaArHqXvGGAs+PxzGbrD14lDUQtKI995Kr0jlog031Tm2htna5fbJDtOOQ==</P><Q>5NbbhRAzZvK2xArnmr3q9u2pjuVCaZ3rvzmy3QWwMHz/RC1V2qLqtHIm2dsTGoOw1nIFZ05XgGbQ9hYcIbkFRQ==</Q><DP>jAx1WuSHOSbpxvo+Phlfk45Rw6Yp73TYN1t5m5p/Y6K5JD90GumCuzZ5SrgJbn2k1lINbKBFpI8J4CK0eDDXAQ==</DP><DQ>KGImvSD6AxvF5lxG/TCS4EgO3PtH9IVHuFV56sTEvUM8GxcPJpU4ejJDF2LlQOoQJdfi1f7EJbfKhceqnIoP/Q==</DQ><InverseQ>J1oZ2hKbqofvZk4R9091t4b/RtuKxUcdUTGCvGzj4hcAKYrcrM0EB87gKLaTq0/WGq+6tnibkHmnDtWGs3yVpg==</InverseQ><D>UHqx8eVexid8OUOJXcIDkm59+T5QUMWfWFWNUGc0W2TJP2Py61WqOw0WN/cNcoXkmVcSFej+M6Yezj94IFE+kbhZ7yc5/tfK3Q34CefArwyc8JWBbz1zHUqJ+G1jlSEfVsd6TrqAxnNbCfOQpUw99vdneBqMDag9U5y5ZwsaSNE=</D></RSAKeyValue>");
			PrivateKey recepientKey = new PrivateKey("<RSAKeyValue><Modulus>nnAPMv712zrTb3nfKbh4jKHMsq1QZYdymeTiisigBSICm1+fHD3rTVC4NTWqKq0ACwhBw8U/ctmHWtM2/R5ayyPd2mdc7kyf4UJlOaPAXQSvjGFkUa4jOUk51ANK9oxxiK7x4JSwp5thE3omLHFX//XMnqAdgFb7aus/+2/Usl8=</Modulus><Exponent>EQ==</Exponent><P>sqSzlRCXLm8QjoivGsXS72N/sTCDOGjmEDQE90jZFLMalkNUP6SmY+Kjj/I/e0s4H/9VQvRuSgHnjOtpiKnTuQ==</P><Q>4wt8gW+VAPo1wi2sd26Kdclkk2OpyWXpGBR708kgdosPbxoVByYGrNdtJS/GTULbwAEZyvNi0ukw5ya7fQfy1w==</Q><DP>naBiODvQsIAdqvES+YFfxCqd2JQ3jCBShsR82jE4AzSe/Q47RzbtKvUm6GxWIX6a7w54aEEWBRC9QBsCw+EkOQ==</DP><DQ>Qscknoo64it5ORx+BQJk9Xd37x1QDg7qNEJCiZWCBL99Ao8zXHSYjTBNRywrQ+Z83h5w/3TCtoDSJd43JMYaPw==</DQ><InverseQ>T0pbLs15abbXCWGj/zOw0DAIt1AHfq2/O6suwJdufb1om96UJSIsivh+MR9ikPOx5XbNIY9Tr2rtBNBTu1XtUA==</InverseQ><D>XTLbw6UJF4wD5zihY9XsjvXDtGXzDovpDztYFWb0t7mnLjg/egZOLXrGteNVCgtpb+a9RfuOvAd8zAPF/k4XSWKxcz6f+bbU0505r84WHeVb6mtNQr+5L16sEtXHK2oeN68MCfGWVyRiPd1NvGNJme60WDfjWAQJmevAnD5u5RE=</D></RSAKeyValue>");
			KeyStorage senderKeyStorage = new MemoryKeyStorage();
			KeyStorage recipientKeyStorage = new MemoryKeyStorage();
			senderKeyStorage.Add("sender", senderKey);
			senderKeyStorage.Add("recepient", recepientKey.PublicKey);
			recipientKeyStorage.Add("sender", senderKey.PublicKey);
			recipientKeyStorage.Add("recepient", recepientKey);

			DiskStorage storage = new DiskStorage("StorageA");

			Console.WriteLine("Keys and storage initialized");

			Console.WriteLine("Sending...");
			Sender send = new Sender("Source", storage, senderKeyStorage, recepientKey.PublicKey);
			send.Run();

			Console.WriteLine("Receiving...");
			Receiver receive = new Receiver(storage, "TargetA", recipientKeyStorage);
			receive.Run();

			Console.WriteLine("Receiving 2...");
			DiskStorage myself = new DiskStorage("StorageB");
			Receiver receiveMyself = new Receiver(myself, "TargetB", recipientKeyStorage);
			receiveMyself.Run();

			Console.WriteLine("All done");

		}

		/// <summary>
		/// Remove all generated directories from previous test
		/// </summary>
		private static void Cleanup()
		{
			string[] dirs = new[] { "StorageA", "StorageB", "TargetA", "TargetB" };
			foreach (string d in dirs)
			{
				if (Directory.Exists(d))
					Directory.Delete(d, true);
			}
		}

	}
}

