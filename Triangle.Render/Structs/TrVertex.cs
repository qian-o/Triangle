using Silk.NET.Maths;
using System.Diagnostics.CodeAnalysis;

namespace Triangle.Render.Structs;

public readonly struct TrVertex(Vector3D<float> position, Vector3D<float> normal, Vector2D<float> texCoords, Vector3D<float> tangent, Vector3D<float> bitangent) : IEquatable<TrVertex>
{
    public Vector3D<float> Position { get; } = position;

    public Vector3D<float> Normal { get; } = normal;

    public Vector2D<float> TexCoords { get; } = texCoords;

    public Vector3D<float> Tangent { get; } = tangent;

    public Vector3D<float> Bitangent { get; } = bitangent;

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Position, Normal, TexCoords, Tangent, Bitangent);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrVertex binding && Equals(binding);
    }

    public readonly bool Equals(TrVertex other)
    {
        return Position.Equals(other.Position)
               && Normal.Equals(other.Normal)
               && TexCoords.Equals(other.TexCoords)
               && Tangent.Equals(other.Tangent)
               && Bitangent.Equals(other.Bitangent);
    }

    public static bool operator ==(TrVertex value1, TrVertex value2)
    {
        return value1.Equals(value2);
    }

    public static bool operator !=(TrVertex value1, TrVertex value2)
    {
        return !value1.Equals(value2);
    }
}