using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;

namespace ExifProcessLib.Processors
{
    public interface IExtractExif
    {
        IEnumerable<ExifData> Extract(Stream stream);
    }
}
