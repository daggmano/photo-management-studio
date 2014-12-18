using System;
using System.Linq;
using System.Text;
using ExifProcessLib.Models;

namespace ExifProcessLib.Helpers
{
    public static class BufferHelper
    {
        private static Endianess ConvertEndianess(Endianess endianess)
        {
            if (BitConverter.IsLittleEndian)
            {
                return endianess == Endianess.Big ? Endianess.Little : Endianess.Big;
            }
            return endianess;
        }

        public static bool CompareBytes(this byte[] data, int offset, params byte[] values)
        {
            if (data.Length < offset + values.Length)
            {
                throw new ArgumentException("Insufficient data provided for comparison");
            }

            return !values.Where((t, i) => data[offset + i] != t).Any();
        }

        public static uint ReadUInt(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 4, ConvertEndianess(endianess));
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static int ReadInt(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 4, ConvertEndianess(endianess));
            return BitConverter.ToInt32(bytes, 0);
        }

        public static float ReadSingle(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 4, ConvertEndianess(endianess));
            return BitConverter.ToSingle(bytes, 0);
        }

        public static double ReadDouble(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 8, ConvertEndianess(endianess));
            return BitConverter.ToDouble(bytes, 0);
        }

        public static ushort ReadUShort(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 2, ConvertEndianess(endianess));
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static short ReadShort(this byte[] data, int offset, Endianess endianess)
        {
            var bytes = ReadData(data, offset, 2, ConvertEndianess(endianess));
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte ReadByte(this byte[] data, int offset)
        {
            return data[offset];
        }

        public static sbyte ReadSByte(this byte[] data, int offset)
        {
            return (sbyte)data[offset];
        }

        public static ExifTag ReadExifType(this byte[] data, int offset, Endianess endianess)
        {
            var value = ReadUShort(data, offset, endianess);
            return (ExifTag)value;
        }

        public static GPSTag ReadGPSType(this byte[] data, int offset, Endianess endianess)
        {
            var value = ReadUShort(data, offset, endianess);
            return (GPSTag)value;
        }

        public static string ReadString(this byte[] data, int offset, uint length)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                if (data[offset + i] != 0x00)
                {
                    sb.Append((char) data[offset + i]);
                }
            }

            return sb.ToString();
        }

        private static byte[] ReadData(byte[] data, int offset, int length, Endianess endianess)
        {
            if (data.Length < offset + length)
            {
                throw new ArgumentException("Insufficient data provided for read");
            }
            var bytes = new byte[length];

            for (var i = 0; i < length; i++)
            {
                var o = endianess == Endianess.Big ? i : length - i - 1;
                bytes[i] = data[o + offset];
            }

            return bytes;
        }
    }
}
