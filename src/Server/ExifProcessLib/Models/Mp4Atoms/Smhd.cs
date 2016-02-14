using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Smhd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public float Balance { get; set; }
		public byte[] Reserved { get; set; }

		public Smhd(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			Balance = data.ReadFixed8(12, Endianess.Big);
			Reserved = data.ReadData(14, 2, Endianess.Big);
		}
	}
}
