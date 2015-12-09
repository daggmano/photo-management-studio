using ExifProcessLib.Helpers;
using System.Collections.Generic;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Trak : Atom
	{
		public List<Atom> Subatoms { get; set; }

		public Trak(byte[] data) : base(data)
		{
			Subatoms = new List<Atom>();

			var idx = 8;
			while (idx < Length)
			{
				var subLength = data.ReadUInt(idx, Endianess.Big);
				var subType = data.ReadString(idx + 4, 4);
				var subData = data.ReadData(idx, (int)subLength, Endianess.Big);

				switch (subType)
				{
					case "moov":
						Subatoms.Add(new Moov(subData));
						break;
					case "mvhd":
						Subatoms.Add(new Mvhd(subData));
						break;
					case "tkhd":
						Subatoms.Add(new Tkhd(subData));
						break;
					case "trak":
						Subatoms.Add(new Trak(subData));
						break;
				}

				idx += (int)subLength;
			}
		}
	}
}
