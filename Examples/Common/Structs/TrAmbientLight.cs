using Silk.NET.Maths;

namespace Common.Structs;

public struct TrAmbientLight(Vector3D<float> color)
{
    public Vector3D<float> Color = color;
}