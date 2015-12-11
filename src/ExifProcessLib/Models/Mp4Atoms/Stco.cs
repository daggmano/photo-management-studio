using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public class Stco : Atom
	{
		public byte Version { get; set; }
		public byte[] Flags { get; set; }
		public uint NumberOfEntries { get; set; }
		public uint[] ChunkOffsets { get; set; }

		public Stco(byte[] data) : base(data)
		{
			Version = data.ReadByte(8);
			Flags = data.ReadData(9, 3, Endianess.Big);
			NumberOfEntries = data.ReadUInt(12, Endianess.Big);

			ChunkOffsets = new uint[NumberOfEntries];
			for (var i = 0; i < NumberOfEntries; i++)
			{
				ChunkOffsets[i] = data.ReadUInt(16 + (i * 4), Endianess.Big);
			}
		}
	}
}
