using System;
using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;
using ExifProcessLib.Processors;

namespace ExifProcessLib
{
    public class ExifProcessor
    {
        private readonly string _fileName;

        public ExifProcessor(string filename)
        {
            _fileName = filename;
        }

        public IEnumerable<IImageData> ProcessImage()
        {
            if (!File.Exists(_fileName))
            {
                throw new ArgumentException("File does not exist");
            }

            using (var stream = File.OpenRead(_fileName))
            {
                IEnumerable<IImageData> result = null;
                var fileType = ImageIdentifier.Identify(_fileName, stream);
                switch (fileType.Type)
                {
                    case ImageType.Jpeg:
                        var jpegExtract = new ExtractExifJpeg();
                        result = jpegExtract.Extract(stream);
                        break;

                    case ImageType.Cr2:
                    case ImageType.Dng:
                        var tiffExtract = new ExtractExifTiff();
                        result = tiffExtract.Extract(stream);
                        break;

					case ImageType.Png:
						var pngExtract = new ExtractDataPng();
						result = pngExtract.Extract(stream);
						break;

					case ImageType.Mp4:
						var mp4Extract = new ExtractDataMp4();
						result = mp4Extract.Extract(stream);
						break;
                }

                return result;
            }
        }
    }
}
