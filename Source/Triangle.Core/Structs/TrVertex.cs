using Silk.NET.Maths;

namespace Triangle.Core.Structs;

public struct TrVertex(Vector3D<float> position = default, Vector3D<float> normal = default, Vector3D<float> tangent = default, Vector3D<float> bitangent = default, Vector4D<float> color = default, Vector2D<float> texCoord = default)
{
    public Vector3D<float> Position = position;

    public Vector3D<float> Normal = normal;

    public Vector3D<float> Tangent = tangent;

    public Vector3D<float> Bitangent = bitangent;

    public Vector4D<float> Color = color;

    public Vector2D<float> TexCoord = texCoord;
}