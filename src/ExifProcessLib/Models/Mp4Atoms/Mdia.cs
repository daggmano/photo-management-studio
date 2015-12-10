using ExifProcessLib.Helpers;
using System.Collections.Generic;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Mdia : Atom
	{
		public List<Atom> Subatoms { get; set; }

		public Mdia(byte[] data) : base(data)
		{
			Subatoms = new List<Atom>();

			var idx = 8;
			while (idx < Length)
			{
				var subLength = data.ReadUInt(idx, Endianess.Big);
				var subData = data.ReadData(idx, (int)subLength, Endianess.Big);

				Subatoms.AddRange(Atom.Decode(subData));

				idx += (int)subLength;
			}
		}
	}
}
