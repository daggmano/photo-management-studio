using System;
using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;
using ExifProcessLib.Helpers;

namespace ExifProcessLib.Processors
{
	public class ExtractDataPng : IExtractExif
	{
		public IEnumerable<ExifData> Extract(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			var buffer = new byte[256];
			var result = new List<ExifData>();

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

				//stream.Read(buffer, 0, 2);
				//if (buffer.CompareBytes(0, 0xFF, 0xDA))
				//{
				//	// Start of image data, so finish.
				//	break;
				//}
				//stream.Read(buffer, 2, 2);
				//// Length of section starts before length bytes
				//stream.Seek(-2, SeekOrigin.Current);
				break;
			}

			return result;
		}
	}
}
