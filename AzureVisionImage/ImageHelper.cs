using System;
using System.IO;
using static System.String;

namespace Shared
{
    public static class ImageHelper
    {
	    public static byte[] GetImageAsByteArray(string imageFilePath)
	    {
		    FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
		    BinaryReader binaryReader = new BinaryReader(fileStream);
		    return binaryReader.ReadBytes((int)fileStream.Length);
	    }

	    public static string GetImageAsBase64String(string imageFilePath)
	    {
		    var base64String = Empty;
			using (var image = System.Drawing.Image.FromFile(imageFilePath))
			{
				using (MemoryStream m = new MemoryStream())
				{
					image.Save(m, image.RawFormat);
					byte[] imageBytes = m.ToArray();

					// Convert byte[] to Base64 String
					base64String = Convert.ToBase64String(imageBytes);
				}
			}
		    return base64String;
		}
	}
}
