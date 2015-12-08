using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;

namespace ExifProcessLib.Processors
{
	public class ExtractDataMp4 : IExtractMp4
	{
		public IEnumerable<Mp4Data> Extract(Stream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);

			var buffer = new byte[256];
			var result = new List<Mp4Data>();

			stream.Seek(0, SeekOrigin.Begin);

			while (true)
			{
				// Atom format is 4 bytes length, 4 byte atom identifier, followed by data.  Length is entire packet including the 8 bytes mentioned.
				stream.Read(buffer, 0, 4);
				var atomLength = (buffer[0] * 256 * 256 * 256) + (buffer[1] * 256 * 256) + (buffer[2] * 256) + buffer[3];

				stream.Read(buffer, 0, 4);
				var atomType = System.Text.Encoding.ASCII.GetString(buffer, 0, 4);

				// We are interested in the moov atom only (and its sub-atoms)
				if (atomType.Equals("moov"))
				{
					var atomData = new byte[atomLength - 8];
					stream.Read(atomData, 0, atomLength - 8);

					result.AddRange(DecodeMoov(atomData));
				}
				else
				{
					stream.Seek(atomLength - 8, SeekOrigin.Current);
				}

				if (stream.Position == stream.Length)
				{
					break ;
				}
			}

			return result;
		}

		private IEnumerable<Mp4Data> DecodeMoov(byte[] atom)
		{
			return new List<Mp4Data>
			{
				new Mp4Data
				{
					Atom = "moov",
					TagName = "Length",
					TagValue = $"{atom.Length} bytes"
				}
			};
		}
	}
}
