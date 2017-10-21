using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DBCloud.ExternalServices.OCR.Model;
using Drawboard.PDF.Engine.DocumentEngine.Models;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json.Linq;
using static System.String;

namespace DBCloud.ExternalServices.OCR
{
	public static class OcrHelper
	{
		public static int WordCountToSkipConversion = 100;

		/// <summary> Convert a pure text string to lines and words but without bouding rectangles </summary>
		/// <param name="data"> The pure text string </param>
		/// <returns> List of OCR Line Results </returns>
		public static List<OcrLineResult> TextToLinesAndWordsNoBounds(string data)
		{
			var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

			var linelist = new List<OcrLineResult>();

			foreach (var line in lines)
			{
				var words = line.Split(new[] { " " }, StringSplitOptions.None);

				var wordlist = ( from word in words
					where !string.IsNullOrWhiteSpace(word)
					select new OcrWordResult()
					{
						Bounds = new double[0],
						Text = word
					} )
					.ToList();

				if (wordlist.Count > 0)
				{
					var ocrline = new OcrLineResult()
					{
						Bounds = new double[0],
						Words = wordlist
					};
					linelist.Add(ocrline);
				}
			}

			return linelist;
		}

		/// <summary> Convert document text structure to OCR text structure </summary>
		public static OcrEngineResult DocumentTextToOcrText(List<DocumentLine> documentlines)
		{
			var ocrResult = new OcrEngineResult();

			foreach (var line in documentlines)
			{
				var ocrLine = new OcrLineResult()
				{
					Bounds = line.Bounds.Array()
				};

				foreach (var word in line.Words)
				{
					var theWord = word.Text().Replace(" ", "");
					if (!string.IsNullOrWhiteSpace(word.Text()))
					{
						var ocrWord = new OcrWordResult()
						{
							Bounds = word.Bounds.Array(),
							Text = theWord
						};
						ocrLine.Words.Add(ocrWord);
					}
				}

				ocrResult.OcrData.Add(ocrLine);
			}

			return ocrResult;
		}

		/// <summary> Convert azure vision string to OCR text structure </summary>
		public static OcrEngineResult AzureVisionStringToOcrText(string input, int pageWidth, int pageHeight)
		{
			var ocrResult = new OcrEngineResult();

			if (IsNullOrWhiteSpace(input))
			{
				ocrResult.Errors.Add("no root data");
				return ocrResult;
			}

			var fulldata = JObject.Parse(input);
			if (fulldata == null)
			{
				ocrResult.Errors.Add("no root data");
				return ocrResult;
			}

			// Error processing OCR from service
			var errorCode = fulldata["code"];
			if (errorCode != null)
			{
				var error = errorCode.Value<string>();
				ocrResult.Errors.Add(error);
				return ocrResult;
			}

			var regions = fulldata["regions"];

			if (regions == null)
			{
				ocrResult.Errors.Add("no region data");
				return ocrResult;
			}

			foreach (var region in regions)
			{
				if (region == null) { continue;}

				var lines = region["lines"];

				if (lines == null)
				{
					ocrResult.Errors.Add("no line data");
					return ocrResult;
				}

				foreach (var line in lines)
				{
					var lineBox = line["boundingBox"];
					var words = line["words"];

					if (lineBox == null || words == null)
					{
						continue;
					}

					var lineBoundStr = lineBox.Value<string>();
					double [] lineBounds = lineBoundStr.Split(',').Select(n => Convert.ToDouble(n)).ToArray();

					var lineX1 = lineBounds[0];
					var lineX2 = lineBounds[0] + lineBounds[2];  // left + width
					var lineY1 = lineBounds[1];
					var lineY2 = lineBounds[1] + lineBounds[3]; // top + height

					var ocrLine = new OcrLineResult()
					{
						Bounds = new double[] { lineX1, lineY1, lineX2, lineY2 }
					};

					foreach (var word in words)
					{
						var wordBox = word["boundingBox"];
						var text = word["text"];
						if (wordBox != null && text != null)
						{
							var wordBoundsStr = wordBox.Value<string>();
							double[] wordBounds = wordBoundsStr.Split(',').Select(n => Convert.ToDouble(n)).ToArray();
							
							var wordX1 = wordBounds[0];
							var wordX2 = wordBounds[0] + wordBounds[2];  // left + width
							var wordY1 = wordBounds[1];
							var wordY2 = wordBounds[1] + wordBounds[3]; // top + height

							var ocrWord = new OcrWordResult()
							{
								Bounds = new double[] { wordX1, wordY1, wordX2, wordY2 },
								Text = text.ToString()
							};
							ocrLine.Words.Add(ocrWord);
						}
					}

					ocrResult.OcrData.Add(ocrLine);
				}
			}

			return ocrResult;
		}

		/// <summary> Parse a bool </summary>
		public static bool SafeParseBool(string input)
		{
			var result = false;
			return bool.TryParse(input, out result) && result;
		}

		/// <summary> Parse an integer </summary>
		public static int SafeParseInt(string input)
		{
			var result = 0;
			return int.TryParse(input, out result) ? result : -1;
		}

		/// <summary> Helper to convert an Abbyy xml result to an OcrEngineResult</summary>
		public static OcrEngineResult AbbyyXmlToOcrText(string filePath)
		{
			var ocrResult = new OcrEngineResult();

			var xdoc = new XmlDocument();
			xdoc.Load(filePath);
			var document = xdoc["document"];

			// invalid document - skip ocr, empty result returned
			if (document?.Attributes == null)
			{
				ocrResult.Errors.Add("Invalid document data");
				return ocrResult;
			}

			var page = document["page"];

			// invalid page - skip ocr, empty result returned
			if (page?.Attributes == null)
			{
				ocrResult.Errors.Add("Invalid page data");
				return ocrResult;
			}

			// Page data
			var pageWidth = page.Attributes.GetNamedItem("width").Value;
			var pageHeight = page.Attributes.GetNamedItem("height").Value;
			var pageResolution = page.Attributes.GetNamedItem("resolution").Value;
			var pageHasOriginalCoords = page.Attributes.GetNamedItem("originalCoords").Value;

			ocrResult.SourcePageWidth = SafeParseInt(pageWidth);
			ocrResult.SourcePageHeight = SafeParseInt(pageHeight);
			ocrResult.SourcePageResolution = SafeParseInt(pageResolution);
			if (pageHasOriginalCoords == "1") ocrResult.IsUsingSourcePageCoords = true;

			using (var lineList = page.GetElementsByTagName("line"))
			{
				// No line data - skip ocr, empty result returned
				if (lineList.Count == 0)
				{
					ocrResult.Errors.Add("Invalid line data");
					return ocrResult;
				}

				foreach (XmlNode line in lineList)
				{
					// Invalid line - skip line & no line added
					if (line?.Attributes == null) { continue; }

					try
					{
						var lineLeft = line.Attributes.GetNamedItem("l").Value;
						var lineRight = line.Attributes.GetNamedItem("r").Value;
						var lineTop = line.Attributes.GetNamedItem("t").Value;
						var lineBottom = line.Attributes.GetNamedItem("b").Value;

						var ocrLine = new OcrLineResult()
						{
							Bounds = new double[] { SafeParseInt(lineLeft), SafeParseInt(lineBottom), SafeParseInt(lineRight), SafeParseInt(lineTop) }
						};

						var formattingElement = line["formatting"];

						// Invalid words - skip words & no line added
						if (formattingElement == null) { continue;}

						using (var wordList = formattingElement.GetElementsByTagName("charParams"))
						{
							foreach (XmlNode word in wordList)
							{
								// Invalid word  - skip word
								if (word?.Attributes == null ) { continue; }

								// "" actually means a space so lets convert that to how it is supposed to be
								if (word.InnerText == "") word.InnerText = " ";

								var wordText = word.InnerText;
								var wordLeft = word.Attributes.GetNamedItem("l").Value;
								var wordRight = word.Attributes.GetNamedItem("r").Value;
								var wordTop = word.Attributes.GetNamedItem("t").Value;
								var wordBottom = word.Attributes.GetNamedItem("b").Value;

								var wordBounds = new double[] { SafeParseInt(wordLeft), SafeParseInt(wordBottom), SafeParseInt(wordRight), SafeParseInt(wordTop) };

								var ocrWord = new OcrWordResult
								{
									Bounds = wordBounds,
									Text = wordText
								};

								if (word.Attributes.GetNamedItem("suspicious") != null && word.Attributes.GetNamedItem("suspicious").Value == "1") { ocrWord.IsSuspicious = true; }
								if (word.Attributes.GetNamedItem("isTab") != null && word.Attributes.GetNamedItem("isTab").Value == "1") { ocrWord.IsTab = true; }

								ocrLine.Words.Add(ocrWord);
							}

							ocrResult.OcrData.Add(ocrLine);
						}
					}
					catch { }


				}
			}

			return ocrResult;
		}

		/// <summary> Helper to write a stream to a file </summary>
		/// <param name="filepath"> Path to the file to write to </param>
		/// <param name="stream"> Stream to write to the file </param>
		public static void WriteStreamToFile(string filepath, MemoryStream stream)
		{
			using (var file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
			{
				var bytes = new byte[stream.Length];
				stream.Read(bytes, 0, (int) stream.Length);
				file.Write(bytes, 0, bytes.Length);
				stream.Close();
			}
		}

		/// <summary> Determine the number of words in the document </summary>
		public static int DocumentWordCount(List<OcrLineResult> ocrWordsByLine)
		{
			var wordCount = 0;

			if (ocrWordsByLine == null)
			{
				return wordCount;
			}

			return ocrWordsByLine.Where(line => line.Words.Any()).Sum(line => line.Words.Count);
		}

		/// <summary> Convert the source data using the transform matrix passed in </summary>
		public static List<OcrLineResult> ConvertBoundsToPsp(double[,] transformMatrix, List<OcrLineResult> sourceData)
		{
			// Calculate Inverse transformation matrix for the image
			var matrix = Matrix<double>.Build.DenseOfArray(transformMatrix);
			var inverseMatrix = matrix.Inverse();

		//	var rotMatrix = Matrix<double>.

			// Convert coordinate system to local coordinates
			foreach (var line in sourceData)
			{
				var lineBl = new double[] { line.Bounds[0], line.Bounds[1], 1.0 };
				var lineBottomLeft = Vector<double>.Build.DenseOfArray(lineBl);

				var lineTr = new double[] { line.Bounds[2], line.Bounds[3], 1.0 };
				var lineTopRight = Vector<double>.Build.DenseOfArray(lineTr);

				var lineBottomLeftTransformed = inverseMatrix * lineBottomLeft;
				var lineTopRightTransformed = inverseMatrix * lineTopRight;

				line.Bounds[0] = lineBottomLeftTransformed[0];
				line.Bounds[1] = lineBottomLeftTransformed[1];
				line.Bounds[2] = lineTopRightTransformed[0];
				line.Bounds[3] = lineTopRightTransformed[1];

				foreach (var word in line.Words)
				{
					var wordBottomLeft = new double[] { word.Bounds[0], word.Bounds[1], 1.0 };
					var wordBottomLeftV = Vector<double>.Build.DenseOfArray(wordBottomLeft);

					var wordTopRight = new double[] { word.Bounds[2], word.Bounds[3], 1.0 };
					var wordTopRightV = Vector<double>.Build.DenseOfArray(wordTopRight);

					var wordBottomLeftTransformed = inverseMatrix * wordBottomLeftV;
					var wordTopRightTransformed = inverseMatrix * wordTopRightV;

					word.Bounds[0] = wordBottomLeftTransformed[0];
					word.Bounds[1] = wordBottomLeftTransformed[1];
					word.Bounds[2] = wordTopRightTransformed[0];
					word.Bounds[3] = wordTopRightTransformed[1];
				}
			}

			return sourceData;
		}
	}
}