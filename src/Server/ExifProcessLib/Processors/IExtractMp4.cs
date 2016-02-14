using ExifProcessLib.Models;
using System.Collections.Generic;
using System.IO;

namespace ExifProcessLib.Processors
{
	public interface IExtractMp4
	{
		IEnumerable<Mp4Data> Extract(Stream stream);
	}
}
