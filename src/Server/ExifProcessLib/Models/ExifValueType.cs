namespace ExifProcessLib.Models
{
    public enum ExifValueType
    {
        UnsignedByte        = 0x01,
        ASCIIString         = 0x02,
        UnsignedShort       = 0x03,
        UnsignedLong        = 0x04,
        UnsignedRational    = 0x05,
        SignedByte          = 0x06,
        Undefined           = 0x07,
        SignedShort         = 0x08,
        SignedLong          = 0x09,
        SignedRational      = 0x0A,
        Single              = 0x0B,
        Double              = 0x0C
    }
}
