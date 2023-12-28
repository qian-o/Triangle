using Silk.NET.Maths;
using System.Diagnostics.CodeAnalysis;

namespace Triangle.Render.Structs;

public struct TrVertex(Vector3D<float> position = default, Vector3D<float> normal = default, Vector2D<float> texCoord = default, Vector3D<float> tangent = default, Vector3D<float> bitangent = default) : IEquatable<TrVertex>
{
    public Vector3D<float> Position = position;

    public Vector3D<float> Normal = normal;

    public Vector2D<float> TexCoord = texCoord;

    public Vector3D<float> Tangent = tangent;

    public Vector3D<float> Bitangent = bitangent;

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Position, Normal, TexCoord, Tangent, Bitangent);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrVertex binding && Equals(binding);
    }

    public readonly bool Equals(TrVertex other)
    {
        return Position.Equals(other.Position)
               && Normal.Equals(other.Normal)
               && TexCoord.Equals(other.TexCoord)
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