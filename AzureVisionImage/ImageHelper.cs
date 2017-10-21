using System.IO;

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
	}
}
