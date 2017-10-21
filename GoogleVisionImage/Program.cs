using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GoogleVisionImage
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Google Vision Services .......");
			MainAsync().Wait();
		}

		static async Task MainAsync()
		{
			using (var service = new GoogleVision())
			{
				// Extract OCR
				Console.WriteLine("Extracting OCR .......");
				var json = await service.OcrRecog("Assets\\test1.png");
				JObject parsed = JObject.Parse(json);
				foreach (var pair in parsed)
				{
					Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
				}
			}
		}
	}
}