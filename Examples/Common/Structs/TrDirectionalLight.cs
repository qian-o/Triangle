using Silk.NET.Maths;

namespace Common.Structs;

public struct TrDirectionalLight(Vector3D<float> direction, Vector3D<float> color)
{
    public Vector3D<float> Direction = direction;

    public Vector3D<float> Color = color;
}
