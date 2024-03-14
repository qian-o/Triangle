using Silk.NET.Assimp;
using Silk.NET.Maths;
using Triangle.Core.Exceptions;
using Triangle.Core.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core.Helpers;

public static unsafe class TrMeshFactory
{
    public static TrMesh CreateCube(this TrContext context, float size = 0.5f)
    {
        TrVertex[] vertices =
        [
            // Back face
            new(new(-size, -size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(0.0f, 0.0f)),
            new(new(size, size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(1.0f, 1.0f)),
            new(new(size, -size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(1.0f, 1.0f)),
            new(new(-size, -size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(0.0f, 0.0f)),
            new(new(-size, size, -size), new(0.0f, 0.0f, -1.0f), texCoord: new(0.0f, 1.0f)),

            // Front face
            new(new(-size, -size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(0.0f, 0.0f)),
            new(new(size, -size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(1.0f, 1.0f)),
            new(new(size, size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(1.0f, 1.0f)),
            new(new(-size, size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(0.0f, 1.0f)),
            new(new(-size, -size, size), new(0.0f, 0.0f, 1.0f), texCoord: new(0.0f, 0.0f)),

            // Left face
            new(new(-size, size, size), new(-1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(-size, size, -size), new(-1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
            new(new(-size, -size, -size), new(-1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(-size, -size, -size), new(-1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(-size, -size, size), new(-1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 0.0f)),
            new(new(-size, size, size), new(-1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),

            // Right face
            new(new(size, size, size), new(1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, -size, -size), new(1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(size, size, -size), new(1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
            new(new(size, -size, -size), new(1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(size, size, size), new(1.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, -size, size), new(1.0f, 0.0f, 0.0f), texCoord: new(0.0f, 0.0f)),

            // Bottom face
            new(new(-size, -size, -size), new(0.0f, -1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(size, -size, -size), new(0.0f, -1.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
            new(new(size, -size, size), new(0.0f, -1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, -size, size), new(0.0f, -1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(-size, -size, size), new(0.0f, -1.0f, 0.0f), texCoord: new(0.0f, 0.0f)),
            new(new(-size, -size, -size), new(0.0f, -1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),

            // Top face
            new(new(-size, size, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(size, size, size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(size, size, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
            new(new(size, size, size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(-size, size, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(-size, size, size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 0.0f))
        ];

        return new(context, $"Cube {size}", vertices, [.. vertices.Select((a, b) => (uint)b)]);
    }

    public static TrMesh CreateCapsule(this TrContext context)
    {
        return AssimpParsing(context, Path.Combine("Resources", "Models", "Capsule.glb"))[0];
    }

    public static TrMesh CreateSphere(this TrContext context)
    {
        const int X_SEGMENTS = 64;
        const int Y_SEGMENTS = 64;

        List<TrVertex> vertices = [];
        List<uint> indices = [];

        for (int x = 0; x <= X_SEGMENTS; x++)
        {
            for (int y = 0; y <= Y_SEGMENTS; y++)
            {
                float xSegment = (float)x / X_SEGMENTS;
                float ySegment = (float)y / Y_SEGMENTS;
                float xPos = (float)(Math.Cos(xSegment * 2.0 * Math.PI) * Math.Sin(ySegment * Math.PI));
                float yPos = (float)Math.Cos(ySegment * Math.PI);
                float zPos = (float)(Math.Sin(xSegment * 2.0 * Math.PI) * Math.Sin(ySegment * Math.PI));

                vertices.Add(new(new(xPos, yPos, zPos), new(xPos, yPos, zPos), texCoord: new(xSegment, ySegment)));
            }
        }

        for (int x = 0; (x < X_SEGMENTS) && (x < Y_SEGMENTS); x++)
        {
            for (int y = 0; (y < X_SEGMENTS) && (y < Y_SEGMENTS); y++)
            {
                indices.Add((uint)((x * (Y_SEGMENTS + 1)) + y));
                indices.Add((uint)(((x + 1) * (Y_SEGMENTS + 1)) + y));
                indices.Add((uint)((x * (Y_SEGMENTS + 1)) + y + 1));

                indices.Add((uint)(((x + 1) * (Y_SEGMENTS + 1)) + y));
                indices.Add((uint)(((x + 1) * (Y_SEGMENTS + 1)) + y + 1));
                indices.Add((uint)((x * (Y_SEGMENTS + 1)) + y + 1));
            }
        }

        return new(context, $"Sphere", [.. vertices], [.. indices]);
    }

    public static TrMesh CreateStar(this TrContext context)
    {
        return AssimpParsing(context, Path.Combine("Resources", "Models", "Star.glb"))[0];
    }

    public static TrMesh CreateCanvas(this TrContext context)
    {
        TrVertex[] vertices =
        [
            new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
            new(new(-1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 0.0f)),
            new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(1.0f, -1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
            new(new(1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
            new(new(-1.0f, 1.0f, 0.0f), new(0.0f, 0.0f, 0.0f), texCoord: new(0.0f, 1.0f))
        ];

        return new(context, "Canvas", vertices, [.. vertices.Select((a, b) => (uint)b)]);
    }

    public static TrMesh[] AssimpParsing(this TrContext context, string file)
    {
        const PostProcessSteps steps = PostProcessSteps.CalculateTangentSpace
                                       | PostProcessSteps.Triangulate
                                       | PostProcessSteps.GenerateNormals
                                       | PostProcessSteps.GenerateSmoothNormals
                                       | PostProcessSteps.GenerateUVCoords
                                       | PostProcessSteps.FlipUVs
                                       | PostProcessSteps.PreTransformVertices;

        using Assimp importer = Assimp.GetApi();
        Scene* scene = importer.ImportFile(file, (uint)steps);

        if (scene == null)
        {
            throw new TrException($"Assimp parsing failed. Error: {importer.GetErrorStringS()}");
        }

        List<TrMesh> meshes = [];

        ProcessNode(scene->MRootNode);

        return [.. meshes];

        void ProcessNode(Node* node)
        {
            for (uint i = 0; i < node->MNumMeshes; i++)
            {
                Mesh* mesh = scene->MMeshes[node->MMeshes[i]];

                meshes.Add(ProcessMesh(mesh));
            }

            for (uint i = 0; i < node->MNumChildren; i++)
            {
                ProcessNode(node->MChildren[i]);
            }
        }

        TrMesh ProcessMesh(Mesh* mesh)
        {
            TrVertex[] vertices = new TrVertex[mesh->MNumVertices];

            for (uint i = 0; i < mesh->MNumVertices; i++)
            {
                vertices[i].Position = (*&mesh->MVertices[i]).ToGeneric();
                vertices[i].Normal = (*&mesh->MNormals[i]).ToGeneric();
                vertices[i].Tangent = (*&mesh->MTangents[i]).ToGeneric();
                vertices[i].Bitangent = (*&mesh->MBitangents[i]).ToGeneric();

                if (mesh->MColors[0] != null)
                {
                    vertices[i].Color = (*&mesh->MColors[0][i]).ToGeneric();
                }

                if (mesh->MTextureCoords[0] != null)
                {
                    Vector3D<float> texCoord = (*&mesh->MTextureCoords[0][i]).ToGeneric();

                    vertices[i].TexCoord = new Vector2D<float>(texCoord.X, texCoord.Y);
                }
            }

            uint[] indices = new uint[mesh->MNumFaces * 3];

            for (uint i = 0; i < mesh->MNumFaces; i++)
            {
                Face face = mesh->MFaces[i];

                for (uint j = 0; j < face.MNumIndices; j++)
                {
                    indices[i * 3 + j] = face.MIndices[j];
                }
            }

            return new(context, $"{file} - {mesh->MName.AsString}", vertices, indices);
        }
    }

    public static void Dispose(this TrMesh[] meshes)
    {
        foreach (TrMesh mesh in meshes)
        {
            mesh.Dispose();
        }
    }
}
