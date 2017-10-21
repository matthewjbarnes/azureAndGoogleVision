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
				var result = String.Empty;

				// Image recog - DOCUMENT_TEXT_DETECTION
				result = await service.ImageRecog("Assets\\test1.png", ImageRecogType.DOCUMENT_TEXT_DETECTION);
				WriteJson(result);

				// Image recog - IMAGE_PROPERTIES
				result = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.IMAGE_PROPERTIES);
				WriteJson(result);

				// Image recog - FACE_DETECTION
				result = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.FACE_DETECTION);
				WriteJson(result);

				// Image recog - SAFE_SEARCH_DETECTION
				result = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.SAFE_SEARCH_DETECTION);
				WriteJson(result);

				// Image recog - LOGO_DETECTION
				result = await service.ImageRecog("Assets\\Microsoft.png", ImageRecogType.LOGO_DETECTION);
				WriteJson(result);

				// Image recog - LABEL_DETECTION
				result = await service.ImageRecog("Assets\\cruiseNow.jpg", ImageRecogType.LABEL_DETECTION);
				WriteJson(result);

				// Image recog - LANDMARK_DETECTION
				result = await service.ImageRecog("Assets\\landmark.jpg", ImageRecogType.LANDMARK_DETECTION);
				WriteJson(result);

				// Image recog - CROP_HINTS
				result = await service.ImageRecog("Assets\\landmark.jpg", ImageRecogType.CROP_HINTS);
				WriteJson(result);
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