using Silk.NET.Maths;

namespace Triangle.Render.Structs;

public struct TrVertex(Vector3D<float> position = default, Vector3D<float> normal = default, Vector2D<float> texCoord = default, Vector3D<float> tangent = default, Vector3D<float> bitangent = default)
{
    public Vector3D<float> Position = position;

    public Vector3D<float> Normal = normal;

    public Vector2D<float> TexCoord = texCoord;

    public Vector3D<float> Tangent = tangent;

    public Vector3D<float> Bitangent = bitangent;
}