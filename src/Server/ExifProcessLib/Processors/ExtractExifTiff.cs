using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;

namespace ExifProcessLib.Processors
{
    public class ExtractExifTiff : IExtractExif
    {
        public IEnumerable<ExifData> Extract(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var result = new List<ExifData>();

            result.AddRange(GetTiffSection(stream, (int)stream.Length));

            return result;
        }

        private IEnumerable<ExifData> GetTiffSection(Stream stream, int length)
        {
            var data = new byte[length];
            stream.Read(data, 0, length);

            var result = ProcessTiffBlock.Process(data);

            return result;
        }
    }
}
