using System.IO;
using ImageMagick;

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
    }
}
