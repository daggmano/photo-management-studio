namespace ExifProcessLib.Models
{
	public class PngData : IImageData
	{
		public string Chunk { get; set; }
		public string TagName { get; set; }
		public string TagValue { get; set; }
	}

	public static class PngDataExtension
	{
		public static string ToDisplayString(this PngData obj)
		{
			return obj.TagValue;
		}
	}
}
