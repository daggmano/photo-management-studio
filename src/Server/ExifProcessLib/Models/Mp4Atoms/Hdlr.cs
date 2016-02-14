using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Hdlr : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public string ComponentType { get; set; }
		public string ComponentSubtype { get; set; }
		public uint ComponentManufacturer { get; set; }
		public uint ComponentFlags { get; set; }
		public uint ComponentFlagsMask { get; set; }
		public string ComponentName { get; set; }

		public Hdlr(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			ComponentType = data.ReadString(12, 4);
			ComponentSubtype = data.ReadString(16, 4);
			ComponentManufacturer = data.ReadUInt(20, Endianess.Big);
			ComponentFlags = data.ReadUInt(24, Endianess.Big);
			ComponentFlagsMask = data.ReadUInt(28, Endianess.Big);

			var remainder = Length - 32;
			ComponentName = data.ReadString(32, remainder);
		}
	}
}
