using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Vmhd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public ushort GraphicsMode { get; set; }
		public ushort[] Opcolor { get; set; }

		public Vmhd(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			GraphicsMode = data.ReadUShort(12, Endianess.Big);
			Opcolor = new[]
			{
				data.ReadUShort(14, Endianess.Big),
				data.ReadUShort(16, Endianess.Big),
				data.ReadUShort(18, Endianess.Big)
			};
		}
	}
}
