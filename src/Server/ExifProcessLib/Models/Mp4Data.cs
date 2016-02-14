namespace ExifProcessLib.Models
{
	public class Mp4Data : IImageData
	{
		public string Atom { get; set; }
		public string TagName { get; set; }
		public string TagValue { get; set; }
	}

	public static class Mp4DataExtension
	{
		public static string ToDisplayString(this Mp4Data obj)
		{
			return obj.TagValue;
		}
	}
}
