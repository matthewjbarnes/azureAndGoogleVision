using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Shared;
using static System.String;

/*
	Input requirements:+
	Supported image formats: JPEG, PNG, and BMP.
	Image file size must be less than 4 MB.
	Image dimensions must be at least 40 x 40, at most 3200 x 3200.

	https://api.cognitive.azure.cn/vision/v1.0/ocr[?language][&detectOrientation ]
	https://dev.cognitive.azure.cn/docs/services/56f91f2d778daf23d8ec6739/operations/56f91f2e778daf14a499e1fc
	*/

namespace AzureVisionImage
{
	/// <summary> Object to wrap up the vision API https://docs.microsoft.com/en-au/azure/cognitive-services/computer-vision/ </summary>
	public class AzureVision
	{
		private readonly HttpClient client;
		private readonly string SubscriptionKey;
		private readonly string uriBase;


		public AzureVision()
		{
			SubscriptionKey = ConfigurationManager.AppSettings["AzureVisionKey"]; 
			client = new HttpClient();
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
			string requestParameters = "language=unk&detectOrientation=true";
			uriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr" + "?" + requestParameters;
		}

		public void Dispose() { }

		/// <summary> Get OCR from a pdf file on disk </summary>
		public async Task<string> ExtractOcr(AzureVisionInput input)
		{
			if (null == input)
			{
				return Empty;
			}

			// Request body. Posts a locally stored JPEG image.
			var byteData = ImageHelper.GetImageAsByteArray(input.Filepath);

			var result = String.Empty;
			using (ByteArrayContent content = new ByteArrayContent(byteData))
			{
				// This example uses content type "application/octet-stream".
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

				// Execute the REST API call.
				var response = await client.PostAsync(uriBase, content);

				// Get the JSON response.
				result = await response.Content.ReadAsStringAsync();
			}

			return result;
		}
	}
}