using System;

namespace DBCloud.ExternalServices.OCR.Model
{
	[Serializable]
	public class OcrWordResult
	{
		/// <summary> The text for the word </summary>
		public string Text { get; set; }

		/// <summary> The bl, tr coords for the word </summary>
		public double[] Bounds { get; set; }

		/// <summary> If the OCR had any ambiguoity when working out this word</summary>
		public bool IsSuspicious { get; set; } = false;

		/// <summary> If this word is a tab </summary>
		public bool IsTab { get; set; } = false;
	}
}