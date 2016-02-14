using ExifProcessLib.Helpers;
using System;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Tkhd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint CreationTime { get; set; }
		public uint ModificationTime { get; set; }
		public uint TrackId { get; set; }
		public byte[] Reserved1 { get; set; }
		public uint Duration { get; set; }
		public byte[] Reserved2 { get; set; }
		public ushort Layer { get; set; }
		public ushort AlternateGroup { get; set; }
		public float Volume { get; set; }
		public byte[] Reserved3 { get; set; }
		public byte[] MatrixStructure { get; set; }
		public float TrackWidth { get; set; }
		public float TrackHeight { get; set; }

		public Tkhd(byte[] data) : base(data)
		{
			if (data.Length != 92)
			{
				throw new ArgumentException("Incorrect data length for tkhd atom");
			}

			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			CreationTime = data.ReadUInt(12, Endianess.Big);
			ModificationTime = data.ReadUInt(16, Endianess.Big);
			TrackId = data.ReadUInt(20, Endianess.Big);
			Reserved1 = data.ReadData(24, 4, Endianess.Big);
			Duration = data.ReadUInt(28, Endianess.Big);
			Reserved2 = data.ReadData(32, 8, Endianess.Big);
			Layer = data.ReadUShort(40, Endianess.Big);
			AlternateGroup = data.ReadUShort(42, Endianess.Big);
			Volume = data.ReadFixed8(44, Endianess.Big);
			Reserved3 = data.ReadData(46, 2, Endianess.Big);
			MatrixStructure = data.ReadData(48, 36, Endianess.Big);
			TrackWidth = data.ReadFixed16(84, Endianess.Big);
			TrackHeight = data.ReadFixed16(88, Endianess.Big);
		}
	}
}
