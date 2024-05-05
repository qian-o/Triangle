using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrPolygon(TrTriangleFace face, TrPolygonMode mode, float lineWidth = 1.0f, float pointSize = 1.0f)
{
    public static TrPolygon Default => new(TrTriangleFace.FrontAndBack, TrPolygonMode.Fill);

    public TrTriangleFace Face = face;

    public TrPolygonMode Mode = mode;

    public float LineWidth = lineWidth;

    public float PointSize = pointSize;

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrPolygon polygon &&
               Face == polygon.Face &&
               Mode == polygon.Mode &&
               LineWidth == polygon.LineWidth &&
               PointSize == polygon.PointSize;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Face, Mode, LineWidth, PointSize);
    }

    public static bool operator ==(TrPolygon left, TrPolygon right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TrPolygon left, TrPolygon right)
    {
        return !(left == right);
    }
}
