using ExifProcessLib.Helpers;
using System.Collections.Generic;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Dref : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint NumberOfEntries { get; set; }
		public List<Atom> DataReferences { get; set; }

		public Dref(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			NumberOfEntries = data.ReadUInt(12, Endianess.Big);

			DataReferences = new List<Atom>();
			var referenceData = data.ReadData(16, data.Length - 16, Endianess.Big);

			DataReferences.AddRange(Atom.Decode(referenceData));
		}
	}
}
