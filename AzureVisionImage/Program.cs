using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBCloud.ExternalServices.AzureVision;
using Newtonsoft.Json.Linq;

namespace AzureVisionImage
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Azure Vision OCR .......");
			MainAsync().Wait();
		}

		static async Task MainAsync()
		{
			// Extract OCR
			var ocrInput = new AzureVisionInput() {Filepath = "Assets\\test1.png"};
			var service = new AzureVision();
			var jsonResult = await service.ExtractOcr(ocrInput);
			JObject parsed = JObject.Parse(jsonResult);
			foreach (var pair in parsed)
			{
				Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
			}

			// Analyse Image
		}
	}
}
