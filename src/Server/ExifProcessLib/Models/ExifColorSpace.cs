namespace ExifProcessLib.Models
{
    public enum ExifColorSpace
    {
        SRGB            = 0x0001,
        AdobeRGB        = 0x0002,
        WideGamutRGB    = 0xFFFD,
        ICCProfile      = 0xFFFE,
        Uncalibrated    = 0xFFFF
    }
}
