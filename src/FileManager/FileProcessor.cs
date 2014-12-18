using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Models;

namespace FileManager
{
    public class FileProcessor
    {
        public Media ProcessFile(string filePath, string rootPath, string importTagId)
        {
            var retval = new Media
            {
                MediaId = Guid.NewGuid().ToString(),
                FullFilePath = filePath,
                LoweredFilePath = filePath.ToLowerInvariant(),
                FileName = Path.GetFileName(filePath)
            };

            retval.TagIds.Add(importTagId);

            // ShotDate - "2008-02-11T17:05:22.000Z"
            // Rating - 0
            // DateAccuracy - 6
            // Caption - ""
            // Rotate - 0
            // Metadata
            //      - tiff:ImageHeight - 768
            //      - exif:Model - Canon EOS 400D DIGITAL
            //      - tiff:ImageWidth - 1024
            //      - exif:ExposureProgram - Program AE
            //      - exif:FNumber - 5.6

            // remember to set $doctype == "media"
        }
    }
}
