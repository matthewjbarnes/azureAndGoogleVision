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
				// Extract OCR
				var json = await service.OcrRecog("Assets\\test1.png");
				WriteJson(json);

				// Analyse Image - Logo
				json = await service.ImageRecog("Assets\\Microsoft.png", "Logo");
				WriteJson(json);

				// Analyse Image - Face Younger
				json = await service.ImageRecog("Assets\\cruiseTopGun.jpg", "Face");
				WriteJson(json);

				// Analyse Image - Face Older (Now)
				json = await service.ImageRecog("Assets\\cruiseNow.jpg", "Face");
				WriteJson(json);

				// Analyse Image - Landmark
				json = await service.ImageRecog("Assets\\landmark.jpg", "Landmark");
				WriteJson(json);
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
