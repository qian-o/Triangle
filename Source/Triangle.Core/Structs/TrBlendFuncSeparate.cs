using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrBlendFuncSeparate(TrBlendFactor srcRGB, TrBlendFactor dstRGB, TrBlendFactor srcAlpha, TrBlendFactor dstAlpha)
{
    public static TrBlendFuncSeparate Default => new(TrBlendFactor.SrcAlpha, TrBlendFactor.OneMinusSrcAlpha, TrBlendFactor.One, TrBlendFactor.Zero);

    public TrBlendFactor SrcRGB = srcRGB;

    public TrBlendFactor DstRGB = dstRGB;

    public TrBlendFactor SrcAlpha = srcAlpha;

    public TrBlendFactor DstAlpha = dstAlpha;

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrBlendFuncSeparate separate
               && SrcRGB == separate.SrcRGB
               && DstRGB == separate.DstRGB
               && SrcAlpha == separate.SrcAlpha
               && DstAlpha == separate.DstAlpha;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(SrcRGB, DstRGB, SrcAlpha, DstAlpha);
    }

    public static bool operator ==(TrBlendFuncSeparate left, TrBlendFuncSeparate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TrBlendFuncSeparate left, TrBlendFuncSeparate right)
    {
        return !(left == right);
    }
}
