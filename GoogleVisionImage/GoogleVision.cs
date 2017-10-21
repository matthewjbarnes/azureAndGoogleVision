using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using Shared;
using static System.String;

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

		/// <summary> URI for Image Analyse service </summary>
		private string getAnalyseUri()
		{
			string requestParameters = "visualFeatures=Categories,Description,Color,Adult,ImageType,Faces&language=en&details=Celebrities";
			return "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze" + "?" + requestParameters;
		}

		/// <summary> URI for OCR service </summary>
		private string getOcrUri()
		{
			return "https://vision.googleapis.com/v1/images:annotate?key=" + SubscriptionKey;
		}


		/// <summary> Get OCR from a input image </summary>
		public async Task<string> OcrRecog(string filepath)
		{
			var imagetxt = ImageHelper.GetImageAsBase64String(filepath);
			var feature = new Feature() {type = "DOCUMENT_TEXT_DETECTION"};
			var imageData = new Image() {content = imagetxt};
			var request = new Request() {image = imageData, features = new List<Feature>() {feature}};
			var rootObj = new RootObject() {requests = new List<Request>(){request}};
			var json = JsonConvert.SerializeObject(rootObj);


			// Execute the REST API call.
			var stringContent = new StringContent(json);
			var uri = getOcrUri();
			var response = await client.PostAsync(uri, stringContent);

			// Get the JSON response.
			var result = await response.Content.ReadAsStringAsync();

			return result;
		}

		/// <summary> Get data from a input image </summary>
		public async Task<string> ImageRecog(string filepath)
		{
			return Empty;
		}
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
