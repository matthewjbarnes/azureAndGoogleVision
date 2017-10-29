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
				while (true)
				{
					Console.WriteLine("1. OCR");
					Console.WriteLine("2. Logo");
					Console.WriteLine("3. Face");
					Console.WriteLine("4. Image Properties");
					Console.WriteLine("5. Landmark");
					Console.WriteLine("6. Racy");
					Console.WriteLine("7. Label");
					Console.WriteLine("8. Crop");

					var result = Console.ReadLine();

					var json = String.Empty;
					switch (result)
					{
						// OCR
						case "1":
							json = await service.ImageRecog("Assets\\test1.png", ImageRecogType.DOCUMENT_TEXT_DETECTION);
							break;

						// Logo
						case "2":
							json = await service.ImageRecog("Assets\\Microsoft.png", ImageRecogType.LOGO_DETECTION);
							break;

						// Face
						case "3":
							json = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.FACE_DETECTION);
							break;

						// Image properties
						case "4":
							json = await service.ImageRecog("Assets\\landmark.jpg", ImageRecogType.IMAGE_PROPERTIES);
							break;

						// Landmark
						case "5":
							json = await service.ImageRecog("Assets\\landmark.jpg", ImageRecogType.LANDMARK_DETECTION);
							break;

						// Safe
						case "6":
							json = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.SAFE_SEARCH_DETECTION);
							break;

						// Label
						case "7":
							json = await service.ImageRecog("Assets\\Microsoft.png", ImageRecogType.LABEL_DETECTION);
							break;

						// Crop
						case "8":
							json = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.CROP_HINTS);
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