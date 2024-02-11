using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public struct TrPolygon(TrTriangleFace face, TrPolygonMode mode)
{
    public static TrPolygon Default => new(TrTriangleFace.Front, TrPolygonMode.Fill);

    public TrTriangleFace Face = face;

    public TrPolygonMode Mode = mode;
}
