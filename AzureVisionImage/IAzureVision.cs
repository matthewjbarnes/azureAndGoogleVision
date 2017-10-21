using System;
using System.Threading.Tasks;
using AzureVisionImage;

namespace DBCloud.ExternalServices.AzureVision
{
	public interface IAzureVision : IDisposable
	{
		/// <summary> Get OCR from a pdf file on disk </summary>
		Task<string> ExtractOcr(AzureVisionInput input, bool compress);
	}
}