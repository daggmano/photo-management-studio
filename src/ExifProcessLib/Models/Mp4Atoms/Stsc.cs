using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class SampleToChunkEntry
	{
		public uint FirstChunk { get; set; }
		public uint SamplesPerChunk { get; set; }
		public uint SampleDescriptionId { get; set; }
	}

	public class Stsc : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint NumberOfEntries { get; set; }
		public SampleToChunkEntry[] Entries { get; set; }

		public Stsc(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			NumberOfEntries = data.ReadUInt(12, Endianess.Big);

			Entries = new SampleToChunkEntry[NumberOfEntries];
			for (var i = 0; i < NumberOfEntries; i++)
			{
				Entries[i] = new SampleToChunkEntry
				{
					FirstChunk = data.ReadUInt(16 + (i * 12), Endianess.Big),
					SamplesPerChunk = data.ReadUInt(20 + (i * 12), Endianess.Big),
					SampleDescriptionId = data.ReadUInt(24 + (i * 12), Endianess.Big)
				};
			}
		}
	}
}
