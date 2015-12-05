using ExifProcessLib.Models;
using System.Collections.Generic;
using System.IO;

namespace ExifProcessLib.Processors
{
	interface IExtractPng
	{
		IEnumerable<PngData> Extract(Stream stream);
	}
}
