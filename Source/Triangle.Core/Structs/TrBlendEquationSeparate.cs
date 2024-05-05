using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrBlendEquationSeparate(TrBlendEquation modeRGB, TrBlendEquation modeAlpha)
{
    public static TrBlendEquationSeparate Default => new(TrBlendEquation.Add, TrBlendEquation.Add);

    public TrBlendEquation ModeRGB = modeRGB;

    public TrBlendEquation ModeAlpha = modeAlpha;

    public override readonly bool Equals(object? obj)
    {
        return obj is TrBlendEquationSeparate separate
               && ModeRGB == separate.ModeRGB
               && ModeAlpha == separate.ModeAlpha;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(ModeRGB, ModeAlpha);
    }

    public static bool operator ==(TrBlendEquationSeparate left, TrBlendEquationSeparate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TrBlendEquationSeparate left, TrBlendEquationSeparate right)
    {
        return !(left == right);
    }
}
