using Triangle.Core;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace Triangle.Render.Helpers;

public static class TrMeshFactory
{
    public static TrMesh CreateCube(this TrContext context, float size = 0.5f)
    {
        TrVertex[] vertices =
        [
            // Front face
            new(new(-size, -size, size), new(0.0f, 0.0f, 1.0f), new(0.0f, 0.0f)),
            new(new(size, -size, size), new(0.0f, 0.0f, 1.0f), new(1.0f, 0.0f)),
            new(new(size, size, size), new(0.0f, 0.0f, 1.0f), new(1.0f, 1.0f)),
            new(new(size, size, size), new(0.0f, 0.0f, 1.0f), new(1.0f, 1.0f)),
            new(new(-size, size, size), new(0.0f, 0.0f, 1.0f), new(0.0f, 1.0f)),
            new(new(-size, -size, size), new(0.0f, 0.0f, 1.0f), new(0.0f, 0.0f)),

            // Back face
            new(new(-size, -size, -size), new(0.0f, 0.0f, -1.0f), new(1.0f, 0.0f)),
            new(new(-size, size, -size), new(0.0f, 0.0f, -1.0f), new(1.0f, 1.0f)),
            new(new(size, size, -size), new(0.0f, 0.0f, -1.0f), new(0.0f, 1.0f)),
            new(new(size, size, -size), new(0.0f, 0.0f, -1.0f), new(0.0f, 1.0f)),
            new(new(size, -size, -size), new(0.0f, 0.0f, -1.0f), new(0.0f, 0.0f)),
            new(new(-size, -size, -size), new(0.0f, 0.0f, -1.0f), new(1.0f, 0.0f)),

            // Top face
            new(new(-size, size, -size), new(0.0f, 1.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(-size, size, size), new(0.0f, 1.0f, 0.0f), new(0.0f, 0.0f)),
            new(new(size, size, size), new(0.0f, 1.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(size, size, size), new(0.0f, 1.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(size, size, -size), new(0.0f, 1.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(-size, size, -size), new(0.0f, 1.0f, 0.0f), new(0.0f, 1.0f)),

            // Bottom face
            new(new(-size, -size, -size), new(0.0f, -1.0f, 0.0f), new(0.0f, 0.0f)),
            new(new(size, -size, -size), new(0.0f, -1.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(size, -size, size), new(0.0f, -1.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(size, -size, size), new(0.0f, -1.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(-size, -size, size), new(0.0f, -1.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(-size, -size, -size), new(0.0f, -1.0f, 0.0f), new(0.0f, 0.0f)),

            // Right face
            new(new(size, -size, -size), new(1.0f, 0.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(size, size, -size), new(1.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(size, size, size), new(1.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(size, size, size), new(1.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(size, -size, size), new(1.0f, 0.0f, 0.0f), new(0.0f, 0.0f)),
            new(new(size, -size, -size), new(1.0f, 0.0f, 0.0f), new(1.0f, 0.0f)),

            // Left face
            new(new(-size, -size, -size), new(-1.0f, 0.0f, 0.0f), new(0.0f, 0.0f)),
            new(new(-size, -size, size), new(-1.0f, 0.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(-size, size, size), new(-1.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(-size, size, size), new(-1.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(-size, size, -size), new(-1.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(-size, -size, -size), new(-1.0f, 0.0f, 0.0f), new(0.0f, 0.0f))
        ];

        return new(context, vertices, vertices.Select((a, b) => (uint)b).ToArray());
    }

    public static TrMesh CreateGrid(this TrContext context)
    {
        TrVertex[] vertices =
        [
            new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f)),
            new(new(-1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 0.0f)),
            new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f)),
            new(new(1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 1.0f)),
            new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f))
        ];

        return new(context, vertices, vertices.Select((a, b) => (uint)b).ToArray());
    }
}
