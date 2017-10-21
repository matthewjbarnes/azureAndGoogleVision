using System;
using System.Collections.Generic;

namespace DBCloud.ExternalServices.OCR.Model
{
	[Serializable]
	public class OcrLineResult
	{
		public List<OcrWordResult> Words { get; set; } = new List<OcrWordResult>();

		public double[] Bounds;

		/// <summary> Get the line as text </summary>
		/// <returns> line as a text string </returns>
		public string Text()
		{
			var result = "";
			foreach (var element in Words)
			{
				if (!string.IsNullOrWhiteSpace(result))
				{
					result += " ";
				}
				result += element.Text;
			}
			return result;
		}
	}
}