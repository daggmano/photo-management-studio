using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ExifProcessLib;
using ExifProcessLib.Models;
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
                FileName = Path.GetFileName(filePath),
                DateAccuracy = 0,
                Rating = 0,
                Caption = String.Empty
            };

            var exifProcessor = new ExifProcessor(Path.Combine(rootPath, filePath));
            var tags = exifProcessor.ProcessImage();

            if (tags != null)
            {
                var ifd0Primary = tags.Where(x => x.IFDType == IFDType.IFD_0_Primary).ToList();
                var ifd1Thumbnail = tags.Where(x => x.IFDType == IFDType.IFD_1_Thumbnail).ToList();
                var exif = tags.Where(x => x.IFDType == IFDType.IFD_Exif).ToList();
                var gps = tags.Where(x => x.IFDType == IFDType.IFD_GPS).ToList();

                // Shot Date
                var tag = GetTag(new[] {ifd0Primary, exif}, ExifTag.DateTimeOriginal, ExifTag.DateTimeDigitized);
                if (tag != null)
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(tag.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
                    {
                        retval.ShotDate = dt;
                        retval.DateAccuracy = 6;
                    }
                }

                // Orientation
                tag = GetTag(new[] {ifd0Primary, ifd1Thumbnail, exif}, ExifTag.Orientation);
                if (tag != null)
                {
                    int rotate;
                    if (Int32.TryParse(tag.Value, out rotate))
                    {
                        retval.Rotate = rotate;
                    }
                }

                // Image Height
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ImageFullHeight, ExifTag.ImageHeight2, ExifTag.ExifImageHeight);
                if (tag != null)
                {
                    int height;
                    if (Int32.TryParse(tag.Value, out height))
                    {
                        var metadata = new Metadata
                        {
                            Group = "exif",
                            Name = "ImageHeight",
                            Value = height.ToString(CultureInfo.InvariantCulture)
                        };
                        retval.Metadata.Add(metadata);
                    }
                }

                // Image Width
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ImageFullWidth, ExifTag.ImageWidth, ExifTag.ImageWidth2, ExifTag.ExifImageWidth);
                if (tag != null)
                {
                    int width;
                    if (Int32.TryParse(tag.Value, out width))
                    {
                        var metadata = new Metadata
                        {
                            Group = "exif",
                            Name = "ImageWidth",
                            Value = width.ToString(CultureInfo.InvariantCulture)
                        };
                        retval.Metadata.Add(metadata);
                    }
                }

                // Manufacturer
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Make);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Make",
                        Value = tag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

                // Camera Model
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Model, ExifTag.Model2, ExifTag.UniqueCameraModel);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Model",
                        Value = tag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

                // Camera Serial
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.SerialNumber, ExifTag.BodySerialNumber, ExifTag.CameraSerialNumber);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Camera Serial",
                        Value = tag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

                // Lens Model
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Lens, ExifTag.LensModel, ExifTag.LensSpecification);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Lens",
                        Value = tag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

                // Lens Serial
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.LensSerialNumber);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Lens Serial",
                        Value = tag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

                // Exposure Program
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureProgram);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Exposure Program",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Exposure Mode
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureMode);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Exposure Mode",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // ISO Speed Rating
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ISOSpeedRatings, ExifTag.ISOSpeed);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "ISO Rating",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Shutter Speed Value
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureTime, ExifTag.ShutterSpeedValue);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Shutter Speed",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Metering Mode
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.MeteringMode);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Metering Mode",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Flash
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Flash);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Flash",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Focal Length
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FocalLength);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Focal Length",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Focal Length in 35mm
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FocalLengthIn35mmFilm);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Focal Length in 35mm",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Artist
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Artist);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Artist",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Copyright
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Copyright);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Copyright",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // FNumber
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FNumber, ExifTag.ApertureValue);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "F Number",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

                // Color Space
                tag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ColorSpace);
                if (tag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Color Space",
                        Value = tag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }
            }

            retval.TagIds.Add(importTagId);

            return retval;

            // remember to set $doctype == "media"
        }

        private ExifData GetTag(IEnumerable<ExifData> list, params ExifTag[] tags)
        {
            return GetTag(new[] {list}, tags);
        }

        private ExifData GetTag(IEnumerable<IEnumerable<ExifData>> lists, params ExifTag[] tags)
        {
            if (lists == null || !lists.Any() || !tags.Any())
            {
                return null;
            }

            var list = lists.SelectMany(x => x.Select(y => y)).ToList();

            return tags.Select(tag => list.FirstOrDefault(x => x.ExifTag == tag)).FirstOrDefault(data => data != null);
        }
    }
}
