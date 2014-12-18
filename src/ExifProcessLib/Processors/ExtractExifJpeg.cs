using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExifProcessLib.Helpers;
using ExifProcessLib.Models;

namespace ExifProcessLib.Processors
{
    public class ExtractExifJpeg : IExtractExif
    {
        public IEnumerable<ExifData> Extract(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[256];
            var result = new List<ExifData>();

            stream.Seek(0, SeekOrigin.Begin);
            // Being JPG, first bytes will be 0xFF 0xD8, but let's do a sanity check...
            stream.Read(buffer, 0, 2);
            if (!buffer.CompareBytes(0, 0xFF, 0xD8))
            {
                throw new ArgumentException("File is not JPEG");
            }

            while (true)
            {
                stream.Read(buffer, 0, 2);
                if (buffer.CompareBytes(0, 0xFF, 0xDA))
                {
                    // Start of image data, so finish.
                    break;
                }
                stream.Read(buffer, 2, 2);
                // Length of section starts before length bytes
                stream.Seek(-2, SeekOrigin.Current);

                var sectionLength = (buffer[2] * 256) + buffer[3];
                // EXIF starts with 0xFF 0xE1
                if (buffer.CompareBytes(0, 0xFF, 0xE1))
                {
                    if (sectionLength > 7)
                    {
                        stream.Read(buffer, 2, 8);
                        stream.Seek(-8, SeekOrigin.Current);
                        // EXIF block has 'Exif' marker followed by 2 null bytes
                        if (buffer.CompareBytes(4, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00))
                        {
                            result.AddRange(GetExifSection(stream, sectionLength));
                        }
                        else
                        {
                            stream.Seek(sectionLength, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        stream.Seek(sectionLength, SeekOrigin.Current);
                    }
                }
                else
                {
                    stream.Seek(sectionLength, SeekOrigin.Current);
                }
            }

            return result;
        }

        private IEnumerable<ExifData> GetExifSection(Stream stream, int length)
        {
            var data = new byte[length];
            stream.Read(data, 0, length);

            // Sanity check
            if (!data.CompareBytes(2, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00))
            {
                throw new ArgumentException("Not an EXIF section");
            }

            data = data.Skip(8).ToArray();

            var result = ProcessTiffBlock.Process(data);

            return result;
        }
    }
}
