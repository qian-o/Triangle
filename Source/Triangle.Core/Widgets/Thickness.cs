using System.Diagnostics.CodeAnalysis;

namespace Triangle.Core.Widgets;

public struct Thickness : IEquatable<Thickness>
{
    public float Left;

    public float Top;

    public float Right;

    public float Bottom;

    public Thickness(float uniformLength)
    {
        Left = uniformLength;
        Top = uniformLength;
        Right = uniformLength;
        Bottom = uniformLength;
    }

    public Thickness(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public readonly bool Equals(Thickness other)
    {
        return Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Thickness other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public static bool operator ==(Thickness left, Thickness right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Thickness left, Thickness right)
    {
        return !(left == right);
    }
}
