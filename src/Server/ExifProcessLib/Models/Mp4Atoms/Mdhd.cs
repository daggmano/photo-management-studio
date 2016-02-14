using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Mdhd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint CreationTime { get; set; }
		public uint ModificationTime { get; set; }
		public uint TimeScale { get; set; }
		public uint Duration { get; set; }
		public ushort Language { get; set; }
		public ushort Quality { get; set; }

		public Mdhd(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			CreationTime = data.ReadUInt(12, Endianess.Big);
			ModificationTime = data.ReadUInt(16, Endianess.Big);
			TimeScale = data.ReadUInt(20, Endianess.Big);
			Duration = data.ReadUInt(24, Endianess.Big);
			Language = data.ReadUShort(28, Endianess.Big);
			Quality = data.ReadUShort(30, Endianess.Big);
		}
	}
}
