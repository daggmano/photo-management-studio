using System;
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
                Caption = string.Empty
            };

            var exifProcessor = new ExifProcessor(Path.Combine(rootPath, filePath));
            var tags = exifProcessor.ProcessImage();
			// Note: Could be ExifData or PngData

            if (tags != null)
            {
				// Deal with EXIF tags
				var exifTags = tags.Where(x => x is ExifData).Cast<ExifData>().ToList();
                var ifd0Primary = exifTags.Where(x => x.IFDType == IFDType.IFD_0_Primary).ToList();
                var ifd1Thumbnail = exifTags.Where(x => x.IFDType == IFDType.IFD_1_Thumbnail).ToList();
                var exif = exifTags.Where(x => x.IFDType == IFDType.IFD_Exif).ToList();
                var gps = exifTags.Where(x => x.IFDType == IFDType.IFD_GPS).ToList();

                // Shot Date
                var exifTag = GetTag(new[] {ifd0Primary, exif}, ExifTag.DateTimeOriginal, ExifTag.DateTimeDigitized);
                if (exifTag != null)
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(exifTag.Value, "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
                    {
                        retval.ShotDate = dt;
                        retval.DateAccuracy = 6;
                    }
                }

				// Orientation
				exifTag = GetTag(new[] {ifd0Primary, ifd1Thumbnail, exif}, ExifTag.Orientation);
                if (exifTag != null)
                {
                    int rotate;
                    if (Int32.TryParse(exifTag.Value, out rotate))
                    {
                        retval.Rotate = rotate;
                    }
                }

				// Image Height
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ImageFullHeight, ExifTag.ImageLength, ExifTag.ImageHeight2, ExifTag.ExifImageHeight);
                if (exifTag != null)
                {
                    int height;
                    if (Int32.TryParse(exifTag.Value, out height))
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
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ImageFullWidth, ExifTag.ImageWidth, ExifTag.ImageWidth2, ExifTag.ExifImageWidth);
                if (exifTag != null)
                {
                    int width;
                    if (Int32.TryParse(exifTag.Value, out width))
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
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Make);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Make",
                        Value = exifTag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

				// Camera Model
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Model, ExifTag.Model2, ExifTag.UniqueCameraModel);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Model",
                        Value = exifTag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

				// Camera Serial
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.SerialNumber, ExifTag.BodySerialNumber, ExifTag.CameraSerialNumber);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Camera Serial",
                        Value = exifTag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

				// Lens Model
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Lens, ExifTag.LensModel, ExifTag.LensSpecification);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Lens",
                        Value = exifTag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

				// Lens Serial
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.LensSerialNumber);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Lens Serial",
                        Value = exifTag.Value
                    };
                    retval.Metadata.Add(metadata);
                }

				// Exposure Program
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureProgram);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Exposure Program",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Exposure Mode
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureMode);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Exposure Mode",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// ISO Speed Rating
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ISOSpeedRatings, ExifTag.ISOSpeed);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "ISO Rating",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Shutter Speed Value
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ExposureTime, ExifTag.ShutterSpeedValue);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Shutter Speed",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Metering Mode
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.MeteringMode);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Metering Mode",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Flash
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Flash);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Flash",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Focal Length
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FocalLength);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Focal Length",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Focal Length in 35mm
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FocalLengthIn35mmFilm);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Focal Length in 35mm",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Artist
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Artist);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Artist",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Copyright
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.Copyright);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Copyright",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// FNumber
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.FNumber, ExifTag.ApertureValue);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "F Number",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Color Space
				exifTag = GetTag(new[] { ifd0Primary, exif }, ExifTag.ColorSpace);
                if (exifTag != null)
                {
                    var metadata = new Metadata
                    {
                        Group = "exif",
                        Name = "Color Space",
                        Value = exifTag.ToDisplayString()
                    };
                    retval.Metadata.Add(metadata);
                }

				// Deal with PNG tags
				var pngTags = tags.Where(x => x is PngData).Cast<PngData>().ToList();
				foreach (var pngTag in pngTags)
				{
					var metadata = new Metadata
					{
						Group = pngTag.Chunk,
						Name = pngTag.TagName,
						Value = pngTag.TagValue
					};
					retval.Metadata.Add(metadata);
				}
            }

            retval.TagIds.Add(importTagId);

            return retval;
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
