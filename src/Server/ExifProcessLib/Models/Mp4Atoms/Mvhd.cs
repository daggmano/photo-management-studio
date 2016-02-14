using ExifProcessLib.Helpers;
using System;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Mvhd : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint CreationTime { get; set; }
		public uint ModificationTime { get; set; }
		public uint TimeScale { get; set; }
		public uint Duration { get; set; }
		public float PreferredRate { get; set; }
		public float PreferredVolume { get; set; }
		public byte[] Reserved { get; set; }
		public byte[] MatrixStructure { get; set; }
		public uint PreviewTime { get; set; }
		public uint PreviewDuration { get; set; }
		public uint PosterTime { get; set; }
		public uint SelectionTime { get; set; }
		public uint SelectionDuration { get; set; }
		public uint CurrentTime { get; set; }
		public uint NextTrackId { get; set; }

		public Mvhd(byte[] data) : base(data)
		{
			if (data.Length != 108)
			{
				throw new ArgumentException("Incorrect data length for mvhd atom");
			}

			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			CreationTime = data.ReadUInt(12, Endianess.Big);
			ModificationTime = data.ReadUInt(16, Endianess.Big);
			TimeScale = data.ReadUInt(20, Endianess.Big);
			Duration = data.ReadUInt(24, Endianess.Big);
			PreferredRate = data.ReadFixed16(28, Endianess.Big);
			PreferredVolume = data.ReadFixed8(32, Endianess.Big);
			Reserved = data.ReadData(34, 10, Endianess.Big);
			MatrixStructure = data.ReadData(44, 36, Endianess.Big);
			PreviewTime = data.ReadUInt(80, Endianess.Big);
			PreviewDuration = data.ReadUInt(84, Endianess.Big);
			PosterTime = data.ReadUInt(88, Endianess.Big);
			SelectionTime = data.ReadUInt(92, Endianess.Big);
			SelectionDuration = data.ReadUInt(96, Endianess.Big);
			CurrentTime = data.ReadUInt(100, Endianess.Big);
			NextTrackId = data.ReadUInt(104, Endianess.Big);
		}
	}
}
