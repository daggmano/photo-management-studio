using ExifProcessLib.Helpers;

namespace ExifProcessLib.Models.Mp4Atoms
{
	public abstract class Atom
	{
		public uint Length { get; set; }
		public string Type { get; set; }

		public Atom(byte[] data)
		{
			Length = data.ReadUInt(0, Endianess.Big);
			Type = data.ReadString(4, 4);
		}
	}
}
