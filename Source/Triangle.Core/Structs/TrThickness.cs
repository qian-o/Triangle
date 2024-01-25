using System.Diagnostics.CodeAnalysis;

namespace Triangle.Core.Structs;

public struct TrThickness : IEquatable<TrThickness>
{
    public float Left;

    public float Top;

    public float Right;

    public float Bottom;

    public TrThickness(float uniformLength)
    {
        Left = uniformLength;
        Top = uniformLength;
        Right = uniformLength;
        Bottom = uniformLength;
    }

    public TrThickness(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly bool Equals(TrThickness other)
    {
        return Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrThickness other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public static bool operator ==(TrThickness left, TrThickness right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TrThickness left, TrThickness right)
    {
        return !(left == right);
    }
}
