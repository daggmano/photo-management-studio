using ExifProcessLib.Helpers;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public abstract class Atom
	{
		public uint Length { get; set; }
		public string Type { get; set; }

		public Atom(byte[] data)
		{
			Length = data.ReadUInt(0, Endianess.Big);
			Type = data.ReadString(4, 4);
		}

		public static IEnumerable<Atom> Decode(byte[] data)
		{
			var idx = 0;
			var result = new List<Atom>();

			while (idx < data.Length)
			{
				var atomLength = data.ReadUInt(idx, Endianess.Big);
				var atomType = data.ReadString(idx + 4, 4);
				var atomData = data.ReadData(idx, (int)atomLength, Endianess.Big);

				switch (atomType)
				{
					case "ctts":
						result.Add(new Ctts(atomData));
						break;
					case "dinf":
						result.Add(new Dinf(atomData));
						break;
					case "dref":
						result.Add(new Dref(atomData));
						break;
					case "hdlr":
						result.Add(new Hdlr(atomData));
						break;
					case "mdhd":
						result.Add(new Mdhd(atomData));
						break;
					case "mdia":
						result.Add(new Mdia(atomData));
						break;
					case "meta":
						break;
					case "minf":
						result.Add(new Minf(atomData));
						break;
					case "moov":
						result.Add(new Moov(atomData));
						break;
					case "mvhd":
						result.Add(new Mvhd(atomData));
						break;
					case "smhd":
						result.Add(new Smhd(atomData));
						break;
					case "stbl":
						result.Add(new Stbl(atomData));
						break;
					case "stco":
						result.Add(new Stco(atomData));
						break;
					case "stsc":
						result.Add(new Stsc(atomData));
						break;
					case "stsd":
						result.Add(new Stsd(atomData));
						break;
					case "stss":
						break;
					case "stsz":
						break;
					case "stts":
						break;
					case "tkhd":
						result.Add(new Tkhd(atomData));
						break;
					case "trak":
						result.Add(new Trak(atomData));
						break;
					case "udta":
						result.Add(new Udta(atomData));
						break;
					case "vmhd":
						result.Add(new Vmhd(atomData));
						break;
					case "Xtra":
						break;
					case "©xyz":
						break;
					case "url ":
						break;
					default:
						Debug.WriteLine($"Missing decode for atom type: {atomType}");
						break;
				}

				idx += (int)atomLength;
			}

			return result;
		}
	}
}
