using System.Collections.Generic;
using DBCloud.ExternalServices.OCR.Model;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace DBCloud.ExternalServices.OCR
{
	public class OcrEngineResult
	{
		/// <summary> The width of the page that OCR results were determine using </summary>
		public int SourcePageWidth { get; set; }

		/// <summary> The height of the page that OCR results were determine using </summary>
		public int SourcePageHeight { get; set; }

		/// <summary> The resolution of the page that OCR results were determine using </summary>
		public int SourcePageResolution { get; set; }

		/// <summary> If the page was calculated based on the source input coords </summary>
		public bool IsUsingSourcePageCoords { get; set; }

		/// <summary> If there was an error converting the page </summary>
		public List<string> Errors { get; set; } = new List<string>();

		/// <summary> The OCR data itself</summary>
		public List<OcrLineResult> OcrData { get; set; } = new List<OcrLineResult>();
	}
}