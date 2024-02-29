using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrPolygon(TrTriangleFace face, TrPolygonMode mode, float lineWidth = 1.0f, float pointSize = 1.0f)
{
    public static TrPolygon Default => new(TrTriangleFace.FrontAndBack, TrPolygonMode.Fill);

    public TrTriangleFace Face = face;

    public TrPolygonMode Mode = mode;

    public float LineWidth = lineWidth;

    public float PointSize = pointSize;
}
