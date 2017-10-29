using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AzureVisionImage
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Azure Vision Services .......");
			MainAsync().Wait();
		}

		static async Task MainAsync()
		{
			using (var service = new AzureVision())
			{
				while (true)
				{
					Console.WriteLine("1. OCR");
					Console.WriteLine("2. Logo");
					Console.WriteLine("3. Cruise Young");
					Console.WriteLine("4. Cruise Now");
					Console.WriteLine("5. Landmark");
	
					var result = Console.ReadLine();

					var json = String.Empty;
					switch (result)
					{
						// OCR
						case "1":
							json = await service.OcrRecog("Assets\\test1.png");
							break;

						// Logo
						case "2":
							json = await service.ImageRecog("Assets\\Microsoft.png", "Logo");
							break;

						// Face Younger
						case "3":
							json = await service.FaceRecog("Assets\\cruiseTopGun.jpg", "Face");
							break;

						// Face Older (Now)
						case "4":
							json = await service.FaceRecog("Assets\\cruiseNow.jpg", "Face");
							break;

						// Landmark
						case "5":
							json = await service.ImageRecog("Assets\\landmark.jpg", "Landmark");
							break;
					}

					// Output result
					WriteJson(json);
				}
			}
		}

		static void WriteJson(string json)
		{
			JObject parsed = JObject.Parse(json);
			foreach (var pair in parsed)
			{
				Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
			}
		}
	}
}
