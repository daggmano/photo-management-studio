using System;
using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;
using ExifProcessLib.Helpers;

namespace ExifProcessLib.Processors
{
	public class ExtractDataPng : IExtractPng
	{
		public IEnumerable<PngData> Extract(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			var buffer = new byte[256];
			var result = new List<PngData>();

			stream.Seek(0, SeekOrigin.Begin);
			// Being PNG, first bytes will be 0x89 0x50 0x4E 0x47 0x0D 0x0A 0x1A 0x0A but let's do a sanity check...
			stream.Read(buffer, 0, 8);
			if (!buffer.CompareBytes(0, 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A))
			{
				throw new ArgumentException("File is not PNG");
			}

			while (true)
			{
				// get chunks, each chunk is 4 bytes (length), 4 bytes (type), length bytes (data), 4 bytes (crc)
				// in particular, process IHDR, iTXt, sRGB, tEXt, tIME, zTXt
				stream.Read(buffer, 0, 4);
				var chunkLength = (buffer[0] * 256 * 256 * 256) + (buffer[1] * 256 * 256) + (buffer[2] * 256) + buffer[3];

				stream.Read(buffer, 0, 4);
				var chunkType = System.Text.Encoding.ASCII.GetString(buffer, 0, 4);

				var chunkData = new byte[chunkLength];
				stream.Read(chunkData, 0, chunkLength);

				stream.Read(buffer, 0, 4);
				var chunkCrc = (buffer[0] * 256 * 256 * 256) + (buffer[1] * 256 * 256) + (buffer[2] * 256) + buffer[3];

				// Not decoded at this stage: PLTE, IDAT, cHRM, gAMA, iCCP, sBIT, bKGD, hIST, tRNS, pHYs, sPLT, iTXt
				switch (chunkType)
				{
					case "IHDR":
						result.AddRange(DecodeIHDR(chunkData));
						break;

					case "sRGB":
						result.AddRange(DecodesRGB(chunkData));
						break;

					case "tEXt":
						result.AddRange(DecodetEXt(chunkData));
						break;

					case "tIME":
						result.AddRange(DecodetIME(chunkData));
						break;

					case "zTXt":
						result.AddRange(DecodezTXt(chunkData));
						break;
				}

				if (chunkType.Equals("IEND"))
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Decoding of IHDR:
		/// 	Width:				4 bytes
		///		Height:				4 bytes
		///		Bit depth:			1 byte
		///		Color type:			1 byte
		///		Compression method: 1 byte
		///		Filter method:		1 byte
		///		Interlace method:	1 byte
		/// </summary>
		/// <param name="chunkData"></param>
		/// <returns></returns>
		private IEnumerable<PngData> DecodeIHDR(byte[] chunkData)
		{
			var result = new List<PngData>();

			var width = chunkData.ReadUInt(0, Endianess.Big);
			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Width",
				TagValue = $"{width}"
			});

			var height = chunkData.ReadUInt(4, Endianess.Big);
			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Height",
				TagValue = $"{height}"
			});

			var bitDepth = chunkData.ReadByte(8);
			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Bit Depth",
				TagValue = $"{bitDepth} bits"
			});

			var colorType = chunkData.ReadByte(9);
			var tagValue = "";
			switch (colorType)
			{
				case 0:
					tagValue = "Grayscale (0)";
					break;
				case 2:
					tagValue = "RGB (2)";
					break;
				case 3:
					tagValue = "Indexed (3)";
					break;
				case 4:
					tagValue = "Grayscale with Alpha (4)";
					break;
				case 6:
					tagValue = "RGB with Alpha (6)";
					break;
				default:
					tagValue = $"Unknown ({colorType})";
					break;
			}

			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Color Type",
				TagValue = $"{tagValue}"
			});

			var compressionMethod = chunkData.ReadByte(10);
			switch (compressionMethod)
			{
				case 0:
					tagValue = "Deflate (0)";
					break;
				default:
					tagValue = $"Unknown ({compressionMethod})";
					break;
			}

			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Compression Method",
				TagValue = $"{tagValue}"
			});

			var filterMethod = chunkData.ReadByte(11);
			switch (filterMethod)
			{
				case 0:
					tagValue = "Adaptive (0)";
					break;
				default:
					tagValue = $"Unknown ({filterMethod})";
					break;
			}

			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Filter Method",
				TagValue = $"{tagValue}"
			});

			var interlaceMethod = chunkData.ReadByte(12);
			switch (interlaceMethod)
			{
				case 0:
					tagValue = "None (0)";
					break;
				case 1:
					tagValue = "Adam7 (1)";
					break;
				default:
					tagValue = $"Unknown ({interlaceMethod})";
					break;
			}

			result.Add(new PngData
			{
				Chunk = "IHDR",
				TagName = "Interlace Method",
				TagValue = $"{tagValue}"
			});

			return result;
		}

		/// <summary>
		/// Decoding of sRGB:
		///     Rendering intent: 1 byte
		/// </summary>
		/// <param name="chunkData"></param>
		/// <returns></returns>
		private IEnumerable<PngData> DecodesRGB(byte[] chunkData)
		{
			var result = new List<PngData>();

			var renderingIntent = chunkData.ReadByte(0);
			var tagValue = "";
			switch (renderingIntent)
			{
				case 0:
					tagValue = "Perceptual (0)";
					break;
				case 1:
					tagValue = "Relative Colorimetric (2)";
					break;
				case 2:
					tagValue = "Saturation (3)";
					break;
				case 3:
					tagValue = "Absolute Colorimetric (4)";
					break;
				default:
					tagValue = $"Unknown ({renderingIntent})";
					break;
			}

			result.Add(new PngData
			{
				Chunk = "sRGB",
				TagName = "Rendering Intent",
				TagValue = $"{tagValue}"
			});

			return result;
		}

		private IEnumerable<PngData> DecodetEXt(byte[] chunkData)
		{
			var result = new List<PngData>();

			var length = chunkData.Length;
			var idx = 0;
			while (chunkData[idx] != 0x00 && idx < length - 2)
			{
				++idx;
			}

			var tagName = System.Text.Encoding.ASCII.GetString(chunkData, 0, idx);
			var tagValue = System.Text.Encoding.ASCII.GetString(chunkData, idx + 1, length - idx - 1);

			result.Add(new PngData
			{
				Chunk = "tEXt",
				TagName = tagName,
				TagValue = tagValue
			});

			return result;
		}

		/// <summary>
		/// Decode tIME:
		///     Year:   2 bytes (complete; for example, 1995, not 95)
		///     Month:  1 byte (1-12)
		///     Day:    1 byte (1-31)
		///     Hour:   1 byte (0-23)
		///     Minute: 1 byte (0-59)
		///     Second: 1 byte (0-60)
		/// </summary>
		/// <param name="chunkData"></param>
		/// <returns></returns>
		private IEnumerable<PngData> DecodetIME(byte[] chunkData)
		{
			var result = new List<PngData>();

			var year = chunkData.ReadUShort(0, Endianess.Big);
			var month = chunkData.ReadByte(2);
			var day = chunkData.ReadByte(3);
			var hour = chunkData.ReadByte(4);
			var minute = chunkData.ReadByte(5);
			var second = chunkData.ReadByte(6);

			var dt = new DateTime(year, month, day, hour, minute, second);
			result.Add(new PngData
			{
				Chunk = "tIME",
				TagName = "Last Modified Time",
				TagValue = dt.ToString()
			});

			return result;
		}

		private IEnumerable<PngData> DecodezTXt(byte[] chunkData)
		{
			var result = new List<PngData>();

			var length = chunkData.Length;
			var idx = 0;
			while (chunkData[idx] != 0x00 && idx < length - 2)
			{
				++idx;
			}

			var tagName = System.Text.Encoding.ASCII.GetString(chunkData, 0, idx);
			var compressionMethod = chunkData.ReadByte(++idx);
			var tagValueBytes = chunkData.ReadData(idx + 1, length - idx - 1, Endianess.Big);
			var tagValue = "<Unable to decompress tag value.>";

			if (compressionMethod == 0x00)
			{
				// Deflate
				try
				{
					var uncompressed = Ionic.Zlib.ZlibStream.UncompressBuffer(tagValueBytes);
					tagValue = System.Text.Encoding.ASCII.GetString(uncompressed);
				}
				catch
				{ }

			}

			result.Add(new PngData
			{
				Chunk = "zTXt",
				TagName = tagName,
				TagValue = tagValue
			});

			return result;
		}
	}
}
