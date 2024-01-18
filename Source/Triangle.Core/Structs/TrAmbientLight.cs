using Silk.NET.Maths;

namespace Triangle.Core.Structs;

public struct TrAmbientLight(Vector3D<float> color)
{
    public Vector3D<float> Color = color;
}