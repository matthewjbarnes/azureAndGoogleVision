using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Shared;
using static System.String;

/*
	Input requirements for OCR :

	Supported image formats: JPEG, PNG, and BMP.
	Image file size must be less than 4 MB.
	Image dimensions must be at least 40 x 40, at most 3200 x 3200.

	https://api.cognitive.azure.cn/vision/v1.0/ocr[?language][&detectOrientation ]
	https://dev.cognitive.azure.cn/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc
	*/

/*
 * Input Requirements for Image: 

	 Supported input methods: Raw image binary in the form of an application/octet stream or image URL.
	Supported image formats: JPEG, PNG, GIF, BMP.
	Image file size: Less than 4 MB.
	Image dimension: Greater than 50 x 50 pixels.
 */

namespace AzureVisionImage
{
	/// <summary> Object to wrap up the vision API https://docs.microsoft.com/en-au/azure/cognitive-services/computer-vision/ </summary>
	public class AzureVision : IDisposable
	{
		private readonly HttpClient client;
		private readonly string SubscriptionKey;


		public AzureVision()
		{
			SubscriptionKey = ConfigurationManager.AppSettings["AzureVisionKey"]; 
			client = new HttpClient();
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
		}

		/// <summary> URI for Image Analyse service </summary>
		private string getFaceUri()
		{
			string requestParameters = "visualFeatures=Categories,Description,Color,Adult,ImageType,Faces&language=en&details=Celebrities";
			return "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze" + "?" + requestParameters;
		}

		/// <summary> URI for Image Analyse service </summary>
		private string getAnalyseUri()
		{
			string requestParameters = "visualFeatures=Categories,Description,Color,Adult,ImageType&language=en";
			return "https://westus.api.cognitive.microsoft.com/vision/v1.0/analyze" + "?" + requestParameters;
		}

		/// <summary> URI for OCR service </summary>
		private string getOcrUri()
		{
			string requestParameters = "language=unk&detectOrientation=true";
			return "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr" + "?" + requestParameters;
		}


		public void Dispose() { }

		/// <summary> Get OCR from a input image </summary>
		public async Task<string> OcrRecog(string filepath)
		{
			Console.WriteLine("Extracting OCR .......");

			if (null == filepath)
			{
				return Empty;
			}

			// Request body. Posts a locally stored JPEG image.
			var byteData = ImageHelper.GetImageAsByteArray(filepath);

			var result = String.Empty;
			using (ByteArrayContent content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

				// Execute the REST API call.
				var response = await client.PostAsync(getOcrUri(), content);

				// Get the JSON response.
				result = await response.Content.ReadAsStringAsync();
			}

			return result;
		}

		/// <summary> Get image classification from a input image </summary>
		public async Task<string> ImageRecog(string filepath, string type)
		{
			Console.WriteLine($"Analysing Image {type}.......");

			if (null == filepath)
			{
				return Empty;
			}

			// Request body. Posts a locally stored JPEG image.
			var byteData = ImageHelper.GetImageAsByteArray(filepath);

			var result = String.Empty;
			using (ByteArrayContent content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

				// Execute the REST API call.
				var response = await client.PostAsync(getAnalyseUri(), content);

				// Get the JSON response.
				result = await response.Content.ReadAsStringAsync();
			}

			return result;
		}

		/// <summary> Get image classification from a input image </summary>
		public async Task<string> FaceRecog(string filepath, string type)
		{
			Console.WriteLine($"Analysing Image {type}.......");

			if (null == filepath)
			{
				return Empty;
			}

			// Request body. Posts a locally stored JPEG image.
			var byteData = ImageHelper.GetImageAsByteArray(filepath);

			var result = String.Empty;
			using (ByteArrayContent content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

				// Execute the REST API call.
				var response = await client.PostAsync(getFaceUri(), content);

				// Get the JSON response.
				result = await response.Content.ReadAsStringAsync();
			}

			return result;
		}
	}
}