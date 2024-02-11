using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrBlendFunc(TrBlendFactor srcRGB, TrBlendFactor dstRGB, TrBlendFactor srcAlpha, TrBlendFactor dstAlpha)
{
    public static TrBlendFunc Default => new(TrBlendFactor.SrcAlpha, TrBlendFactor.OneMinusSrcAlpha, TrBlendFactor.One, TrBlendFactor.Zero);

    public TrBlendFactor SrcRGB = srcRGB;

    public TrBlendFactor DstRGB = dstRGB;

    public TrBlendFactor SrcAlpha = srcAlpha;

    public TrBlendFactor DstAlpha = dstAlpha;
}
