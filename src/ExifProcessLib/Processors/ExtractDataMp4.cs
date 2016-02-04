using System;
using System.Collections.Generic;
using System.IO;
using ExifProcessLib.Models;
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

					result.AddRange(DecodeAtom(atom));
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
		
		private IEnumerable<Mp4Data> DecodeAtom(Atom atom)
		{
			var result = new List<Mp4Data>();
			switch (atom.Type)
			{
				case "mvhd":
					var mvhd = atom as Mvhd;
					result.Add(new Mp4Data
					{
						Atom = "mvhd",
						TagName = "Duration", 
						TagValue = $"{(decimal)mvhd.Duration / (decimal)mvhd.TimeScale} seconds"
					});
					result.Add(new Mp4Data
					{
						Atom = "mvhd",
						TagName = "Creation Time",
						TagValue = GetDateTime(mvhd.CreationTime)
					});
					result.Add(new Mp4Data
					{
						Atom = "mvhd",
						TagName = "Modification Time",
						TagValue = GetDateTime(mvhd.ModificationTime)
					});
					break;
					
				case "trak":
					var tkhd = GetSubAtom(atom, "tkhd") as Tkhd;
					if (tkhd == null)
					{
						break;
					}
					var mdia = GetSubAtom(atom, "mdia") as Mdia;
					if (mdia == null)
					{
						break;
					}
					var hdlr = GetSubAtom(mdia, "hdlr") as Hdlr;
					if (hdlr != null)
					{
						if (hdlr.ComponentSubtype.Equals("vide"))
						{
							result.Add(new Mp4Data
							{
								Atom = "tkhd",
								TagName = "Track Width",
								TagValue = tkhd.TrackWidth.ToString()
							});
							result.Add(new Mp4Data
							{
								Atom = "tkhd",
								TagName = "Track Height",
								TagValue = tkhd.TrackHeight.ToString()
							});
						}
					}
					break;
			}
			
			var atomWithSubatoms = atom as IAtomWithSubatoms;
			if (atomWithSubatoms != null && atomWithSubatoms.Subatoms != null)
			{
				foreach (var subatom in atomWithSubatoms.Subatoms)
				{
					result.AddRange(DecodeAtom(subatom));
				}
			}
			
			return result;
		}
		
		private string GetDateTime(uint seconds)
		{
			// Time is number of seconds since 1 Jan 1904
			var dateTime = new DateTime(1904, 1, 1, 0, 0, 0);
			dateTime = dateTime.AddSeconds(seconds);
			
			return dateTime.ToString("o");
		}
		
		private Atom GetSubAtom(Atom atom, string type)
		{
			var atomWithSubatoms = atom as IAtomWithSubatoms;
			if (atomWithSubatoms == null || atomWithSubatoms.Subatoms == null)
			{
				return null;
			}
			
			foreach (var subatom in atomWithSubatoms.Subatoms)
			{
				if (subatom.Type.Equals(type))
				{
					return subatom;
				}
				
				var subsubatom = GetSubAtom(subatom, type);
				if (subsubatom != null)
				{
					return subsubatom;
				}
			}
			
			return null;
		}
	}
}
