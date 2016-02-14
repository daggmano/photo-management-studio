using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class CompositionOffsetEntry
	{
		public uint SampleCount { get; set; }
		public int CompositionOffset { get; set; }
	}

	public class Ctts : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint EntryCount { get; set; }
		public CompositionOffsetEntry[] Entries { get; set; }

		public Ctts(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			EntryCount = data.ReadUInt(12, Endianess.Big);
			Entries = new CompositionOffsetEntry[EntryCount];
			for (var i = 0; i < EntryCount; i++)
			{
				Entries[i] = new CompositionOffsetEntry
				{
					SampleCount = data.ReadUInt(16 + (i * 8), Endianess.Big),
					CompositionOffset = data.ReadInt(20 + (i * 8), Endianess.Big)
				};
			}
		}
	}
}
