using System.IO;
using ImageMagick;
using System.Reflection;

namespace PhotoLibraryImageService.Services
{
    public class ImageResizeService
    {
        public static void ProcessImage(string imageId, int maxDimension, string outPath)
        {
            var path = Path.Combine(SettingsService.GetLibraryPath(), imageId);

            if (!File.Exists(path))
            {
                return;
            }

            using (var image = new MagickImage(path))
            {
                image.Resize(maxDimension, maxDimension);
                image.Write(outPath);
            }
        }

		public static byte[] ProcessImage(string path, int maxDimension)
		{
			try {
				if (!File.Exists(path))
				{
					return null;
				}

				using (var image = new MagickImage(path))
				using (var stream = new MemoryStream())
				{
					image.Resize(maxDimension, maxDimension);
					image.Write(stream, MagickFormat.Jpg);

					var result = stream.ToArray();
					return result;
				}
			}
			catch
			{
				var placeholderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				placeholderPath = Path.Combine(placeholderPath, "placeholder-1200x1080.jpg");

				using (var image = new MagickImage(placeholderPath))
				using (var stream = new MemoryStream())
				{
					image.Resize(maxDimension, maxDimension);
					image.Write(stream, MagickFormat.Jpg);

					var result = stream.ToArray();
					return result;
                }
			}
		}
    }
}
