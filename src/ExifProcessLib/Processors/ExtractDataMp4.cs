using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExifProcessLib.Models;
using ExifProcessLib.Helpers;
using System.Diagnostics;
using ExifProcessLib.Models.Mp4Atoms;

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
				// TODO: Should use ReadUInt...
				var atomLength = (buffer[0] * 256 * 256 * 256) + (buffer[1] * 256 * 256) + (buffer[2] * 256) + buffer[3];

				stream.Read(buffer, 0, 4);
				var atomType = System.Text.Encoding.ASCII.GetString(buffer, 0, 4);

				// We are interested in the moov atom only (and its sub-atoms)
				if (atomType.Equals("moov"))
				{
					var atomData = new byte[atomLength];
					stream.Seek(-8, SeekOrigin.Current);
					stream.Read(atomData, 0, atomLength);

					var atom = new Moov(atomData);
					Debugger.Break();
//					result.AddRange(DecodeAtom(atomData, atomType));
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
	}
}
