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
			// Extract OCR
			Console.WriteLine("Extracting OCR .......");
			using (var service = new AzureVision())
			{
				var json = await service.OcrRecog("Assets\\test1.png");
				JObject parsed = JObject.Parse(json);
				foreach (var pair in parsed)
				{
					Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
				}

				// Analyse Image
				Console.WriteLine("Analysing Image .......");
				json = await service.ImageRecog("Assets\\Microsoft.png");
				parsed = JObject.Parse(json);
				foreach (var pair in parsed)
				{
					Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
				}
			}
		}
	}
}
