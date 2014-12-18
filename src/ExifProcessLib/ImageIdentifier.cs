using System.IO;
using ExifProcessLib.Helpers;
using ExifProcessLib.Models;

namespace ExifProcessLib
{
    public static class ImageIdentifier
    {
        public static ImageIdentification Identify(string filename, Stream stream)
        {
            var result = new ImageIdentification
            {
                Type = ImageType.Unknown,
                Endianess = Endianess.Unknown
            };

            var extension = Path.GetExtension(filename.ToLowerInvariant());
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    result.Type = ImageType.Jpeg;
                    break;

                case ".png":
                    result.Type = ImageType.Png;
                    break;

                case ".tif":
                case ".tiff":
                    result.Type = ImageType.Tiff;
                    break;

                case ".cr2":
                    result.Type = ImageType.Cr2;
                    break;

                case ".dng":
                    result.Type = ImageType.Dng;
                    break;

                case ".mov":
                    result.Type = ImageType.Mov;
                    break;
            }

            var buffer = new byte[256];

            switch (result.Type)
            {
                case ImageType.Jpeg:
                    // JPEG starts with 0xFF 0xD8, ends with 0xFF 0xD9
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, 2);
                    if (!buffer.CompareBytes(0, 0xFF, 0xD8))
                    {
                        result.Type = ImageType.Unknown;
                    }
                    else
                    {
                        stream.Seek(-2, SeekOrigin.End);
                        stream.Read(buffer, 0, 2);
                        if (!buffer.CompareBytes(0, 0xFF, 0xD9))
                        {
                            result.Type = ImageType.Unknown;
                        }
                    }
                    break;

                case ImageType.Png:
                    // PNG starts with 0x89 0x50 0x4E 0x47 0x0D 0x0A 0x1A 0x0A
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, 8);
                    if (!buffer.CompareBytes(0, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A))
                    {
                        result.Type = ImageType.Unknown;
                    }
                    break;

                case ImageType.Tiff:
                case ImageType.Cr2:
                case ImageType.Dng:
                    // TIFF / CR2 / DNG start with 0x49 0x49 0x2A 0x00 (little endian) or 0x4D 0x4D 0x00 0x2A (big endian)
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, 4);
                    if (buffer.CompareBytes(0, 0x49, 0x49, 0x2A, 0x00))
                    {
                        result.Endianess = Endianess.Little;
                    }
                    else if (buffer.CompareBytes(0, 0x4D, 0x4D, 0x00, 0x2A))
                    {
                        result.Endianess = Endianess.Big;
                    }
                    else
                    {
                        result.Type = ImageType.Unknown;
                    }
                    break;
                case ImageType.Mov:
                    // For now, assume MOV will be ftyp type, and start with <4-byte length> 0x66 0x74 0x79 0x70
                    stream.Seek(4, SeekOrigin.Begin);
                    stream.Read(buffer, 0, 4);
                    if (!buffer.CompareBytes(0, 0x66, 0x74, 0x79, 0x70))
                    {
                        result.Type = ImageType.Unknown;
                    }
                    break;
            }

            return result;
        }
    }
}
