using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;

namespace GoogleVisionImage
{
	/*
		https://cloud.google.com/vision/docs/
		https://cloud.google.com/functions/docs/tutorials/ocr
	  */

	public class GoogleVision : IDisposable
	{
		private readonly HttpClient client;
		private readonly string SubscriptionKey;

		public GoogleVision()
		{
			SubscriptionKey = ConfigurationManager.AppSettings["GoogleVisionKey"];
			client = new HttpClient();
		}

		public void Dispose()
		{
		}

		/// <summary> URI for OCR service </summary>
		private string Uri()
		{
			return "https://vision.googleapis.com/v1/images:annotate?key=" + SubscriptionKey;
		}

		/// <summary> Get data from a input image </summary>
		public async Task<string> ImageRecog(string filepath, string recogType)
		{
			Console.WriteLine($"Extracting Image Regoc for {recogType} .......");

			var imagetxt = ImageHelper.GetImageAsBase64String(filepath);
			var feature = new Feature() { type = recogType };
			var imageData = new Image() { content = imagetxt };
			var request = new Request() { image = imageData, features = new List<Feature>() { feature } };
			var rootObj = new RootObject() { requests = new List<Request>() { request } };
			var json = JsonConvert.SerializeObject(rootObj);

			// Execute the REST API call.
			var stringContent = new StringContent(json);
			var response = await client.PostAsync(Uri(), stringContent);

			// Get the JSON response.
			var result = await response.Content.ReadAsStringAsync();

			return result;
		}
	}

	public static class ImageRecogType
	{
		public static string IMAGE_PROPERTIES = "IMAGE_PROPERTIES";
		public static string FACE_DETECTION = "FACE_DETECTION";
		public static string LOGO_DETECTION = "LOGO_DETECTION";
		public static string LABEL_DETECTION = "LABEL_DETECTION";
		public static string CROP_HINTS = "CROP_HINTS";
		public static string LANDMARK_DETECTION = "LANDMARK_DETECTION";
		public static string SAFE_SEARCH_DETECTION = "SAFE_SEARCH_DETECTION";
		public static string DOCUMENT_TEXT_DETECTION = "DOCUMENT_TEXT_DETECTION";
	}


	public class Image
	{
		public string content { get; set; }
	}

	public class Feature
	{
		public string type { get; set; }
	}

	public class Request
	{
		public Image image { get; set; }
		public List<Feature> features { get; set; }
	}

	public class RootObject
	{
		public List<Request> requests { get; set; }
	}
}
