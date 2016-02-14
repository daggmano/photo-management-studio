namespace ExifProcessLib.Models
{
    public enum ExifFlash
    {
        NoFlash                             = 0x00,
        Fired                               = 0x01,
        FiredNoReturn                       = 0x05,
        FiredReturn                         = 0x07,
        OnDidNotFire                        = 0x08,
        OnFired                             = 0x09,
        OnNoReturn                          = 0x0D,
        OnReturn                            = 0x0F,
        OffDidNotFire                       = 0x10,
        OffDidNotFireNoReturn               = 0x14,
        AutoDidNotFire                      = 0x18,
        AutoFired                           = 0x19,
        AutoFiredNoReturn                   = 0x1D,
        AutoFiredReturn                     = 0x1F,
        NoFlashFunction                     = 0x20,
        OffNoFlashFunction                  = 0x30,
        FiredRedEyeReduction                = 0x41,
        FiredRedEyeReductionNoReturn        = 0x45,
        FiredRedEyeReductionReturn          = 0x47,
        OnRedEyeReduction                   = 0x49,
        OnRedEyeReductionNoReturn           = 0x4D,
        OnRedEyeReductionReturn             = 0x4F,
        OffRedEyeReduction                  = 0x50,
        AutoDidNotFireRedEyeReduction       = 0x58,
        AutoFiredRedEyeReduction            = 0x59,
        AutoFiredRedEyeReductionNoReturn    = 0x5D,
        AutoFiredRedEyeReductionReturn      = 0x5F
    }
}
