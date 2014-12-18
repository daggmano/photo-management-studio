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

        public IEnumerable<ExifData> ProcessImage()
        {
            if (!File.Exists(_fileName))
            {
                throw new ArgumentException("File does not exist");
            }

            using (var stream = File.OpenRead(_fileName))
            {
                IEnumerable<ExifData> result = null;
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
                }

                return result;
            }
        }
    }
}
