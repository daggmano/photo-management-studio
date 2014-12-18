using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ExifProcessLib.Helpers;
using ExifProcessLib.Models;

namespace ExifProcessLib.Processors
{
    public static class ProcessTiffBlock
    {
        public static IEnumerable<ExifData> Process(byte[] data)
        {
            var result = new List<ExifData>();

            Endianess endianess;
            if (data.CompareBytes(0, 0x49, 0x49, 0x2A, 0x00))
            {
                endianess = Endianess.Little;
            }
            else if (data.CompareBytes(0, 0x4D, 0x4D, 0x00, 0x2A))
            {
                endianess = Endianess.Big;
            }
            else
            {
                throw new ArgumentException("Unable to determine endianess, may not be a TIFF header");
            }

            var position = (int)data.ReadUInt(4, endianess);
            var entryCount = data.ReadUShort(position, endianess);
            position += 2;
            var ifdType = IFDType.IFD_0_Primary;

            while (true)
            {
                for (var i = 0; i < entryCount; i++)
                {
                    var exif = ProcessExifTag(data, position, endianess, ifdType);
                    result.Add(exif);

                    position += 12;
                }

                var nextIfdOffset = data.ReadUInt(position, endianess);
                if (nextIfdOffset != 0)
                {
                    position = (int)nextIfdOffset;
                    entryCount = data.ReadUShort(position, endianess);
                    position += 2;
                    ifdType = IFDType.IFD_1_Thumbnail;
                }
                else
                {
                    break;
                }
            }

            var exifOffsets = result.Where(x => x.ExifTag == ExifTag.ExifOffset).Select(x => UInt32.Parse(x.Value)).ToArray();
            for (var i = 0; i < exifOffsets.Length; i++)
            {
                position = (int)exifOffsets[i];
                entryCount = data.ReadUShort(position, endianess);
                position += 2;

                while (true)
                {
                    for (var j = 0; j < entryCount; j++)
                    {
                        var exif = ProcessExifTag(data, position, endianess, IFDType.IFD_Exif);
                        result.Add(exif);

                        position += 12;
                    }

                    var nextIfdOffset = data.ReadUInt(position, endianess);
                    if (nextIfdOffset != 0)
                    {
                        position = (int)nextIfdOffset;
                        entryCount = data.ReadUShort(position, endianess);
                        position += 2;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var gpsOffsets = result.Where(x => x.ExifTag == ExifTag.GPSInfo).Select(x => UInt32.Parse(x.Value)).ToArray();
            for (var i = 0; i < gpsOffsets.Length; i++)
            {
                position = (int)gpsOffsets[i];
                entryCount = data.ReadUShort(position, endianess);
                position += 2;

                while (true)
                {
                    for (var j = 0; j < entryCount; j++)
                    {
                        var exif = ProcessExifTag(data, position, endianess, IFDType.IFD_GPS);
                        result.Add(exif);

                        position += 12;
                    }

                    var nextIfdOffset = data.ReadUInt(position, endianess);
                    if (nextIfdOffset != 0)
                    {
                        position = (int)nextIfdOffset;
                        entryCount = data.ReadUShort(position, endianess);
                        position += 2;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private static ExifData ProcessExifTag(byte[] data, int position, Endianess endianess, IFDType ifdType)
        {
            var result = new ExifData
            {
                IFDType = ifdType
            };
            if (ifdType == IFDType.IFD_GPS)
            {
                result.GPSTag = data.ReadGPSType(position, endianess);
            }
            else
            {
                result.ExifTag = data.ReadExifType(position, endianess);
            }

            var dataType = data.ReadUShort(position + 2, endianess);
            result.ValueType = (ExifValueType) dataType;
            var componentCount = data.ReadUInt(position + 4, endianess);
            int offset;

            switch (result.ValueType)
            {
                case ExifValueType.UnsignedByte:    // unsigned byte, 1 byte / component
                    if (componentCount > 4)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<byte>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadByte(offset + i));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.ASCIIString:     // ascii strings, 1 byte / component
                    if (componentCount > 4)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    result.Value = data.ReadString(offset, componentCount);
                    break;

                case ExifValueType.UnsignedShort:   // unsigned short, 2 bytes / component
                    if (componentCount > 2)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<ushort>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadUShort(offset + (i * 2), endianess));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.UnsignedLong:    // unsigned long, 4 bytes / component
                    if (componentCount > 1)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<uint>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadUInt(offset + (i * 4), endianess));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.UnsignedRational:    // unsigned rational, 8 bytes / component
                    offset = (int)data.ReadUInt(position + 8, endianess);
                    {
                        var lst = new List<string>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            var numerator = data.ReadUInt(offset + (i * 8), endianess);
                            var denominator = data.ReadUInt(offset + (i * 8) + 4, endianess);
                            lst.Add(String.Format("{0}/{1}", numerator, denominator));
                        }
                        result.Value = String.Join(", ", lst);
                    }
                    break;

                case ExifValueType.SignedByte:      // signed byte, 1 byte / component
                    if (componentCount > 4)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<sbyte>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadSByte(offset + i));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.Undefined:     // undefined
                    switch (result.ExifTag)
                    {
                        case ExifTag.ComponentConfiguration:
                            result.Value = String.Format("{0}, {1}, {2}, {3}",
                                data.ReadByte(position + 8),
                                data.ReadByte(position + 9),
                                data.ReadByte(position + 10),
                                data.ReadByte(position + 11));
                            break;

                        case ExifTag.ExifVersion:
                            result.Value = data.ReadString(position + 8, 4);
                            break;

                        default:
                            {
                                if (componentCount > 4)
                                {
                                    offset = (int)data.ReadUInt(position + 8, endianess);
                                }
                                else
                                {
                                    offset = position + 8;
                                }
                                var lst = new List<ushort>();
                                for (var i = 0; i < componentCount; i++)
                                {
                                    lst.Add(data.ReadUShort(offset + (i * 2), endianess));
                                }
                                result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                            }
                            break;
                    }
                    break;

                case ExifValueType.SignedShort:     // signed short, 2 bytes / component
                    if (componentCount > 2)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<short>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadShort(offset + (i * 2), endianess));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.SignedLong:     // signed long, 4 bytes / component
                    if (componentCount > 1)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<int>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadInt(offset + (i * 4), endianess));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.SignedRational:    // signed rational, 8 bytes / component
                    offset = (int)data.ReadUInt(position + 8, endianess);
                    {
                        var lst = new List<string>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            var numerator = data.ReadInt(offset + (i * 8), endianess);
                            var denominator = data.ReadInt(offset + (i * 8) + 4, endianess);
                            lst.Add(String.Format("{0}/{1}", numerator, denominator));
                        }
                        result.Value = String.Join(", ", lst);
                    }
                    break;

                case ExifValueType.Single:    // single float, 4 bytes / component
                    if (componentCount > 1)
                    {
                        offset = (int)data.ReadUInt(position + 8, endianess);
                    }
                    else
                    {
                        offset = position + 8;
                    }
                    {
                        var lst = new List<float>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadSingle(offset + (i * 4), endianess));
                        }
                        result.Value = String.Join(", ", lst.Select(x => x.ToString(CultureInfo.InvariantCulture)));
                    }
                    break;

                case ExifValueType.Double:    // double float, 8 bytes / component
                    offset = (int)data.ReadUInt(position + 8, endianess);
                    {
                        var lst = new List<double>();
                        for (var i = 0; i < componentCount; i++)
                        {
                            lst.Add(data.ReadDouble(offset + (i * 4), endianess));
                        }
                        result.Value = String.Join(", ", lst);
                    }
                    break;
            }

            return result;
        }

    }
}
