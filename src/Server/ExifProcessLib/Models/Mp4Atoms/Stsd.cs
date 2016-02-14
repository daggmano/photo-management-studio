using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Stsd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint NumberOfEntries { get; set; }

		public Stsd(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			NumberOfEntries = data.ReadUInt(12, Endianess.Big);

			// TODO: Continue to decode this atom
		}
	}
}
