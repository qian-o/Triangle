using Silk.NET.Assimp;
using Silk.NET.Maths;
using Triangle.Core.Exceptions;
using Triangle.Core.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core.Helpers;

public static unsafe class TrMeshFactory
{
    private static readonly Dictionary<string, TrMesh> _meshes = [];
    private static readonly Dictionary<string, Tuple<TrMesh[], TrMaterialProperty[]>> _models = [];

    public static TrMesh GetCube(this TrContext context, float size = 0.5f)
    {
        string name = $"Cube {size}";

        if (!_meshes.TryGetValue(name, out TrMesh? mesh))
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

            mesh = new(context, name, vertices, [.. vertices.Select((a, b) => (uint)b)]);

            _meshes.Add(name, mesh);
        }

        return mesh;
    }

    public static TrMesh GetCapsule(this TrContext context)
    {
        return AssimpParsing(context, Path.Combine("Resources", "Models", "Capsule.glb")).Meshes[0];
    }

    public static TrMesh GetSphere(this TrContext context, float radius = 0.5f)
    {
        string name = $"Sphere {radius}";

        if (!_meshes.TryGetValue(name, out TrMesh? mesh))
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

                    vertices.Add(new(new(xPos * radius, yPos * radius, zPos * radius), new(xPos, yPos, zPos), texCoord: new(xSegment, ySegment)));
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

            mesh = new(context, name, [.. vertices], [.. indices]);

            _meshes.Add(name, mesh);
        }

        return mesh;
    }

    public static TrMesh GetStar(this TrContext context)
    {
        return AssimpParsing(context, Path.Combine("Resources", "Models", "Star.glb")).Meshes[0];
    }

    public static TrMesh GetCanvas(this TrContext context)
    {
        string name = "Canvas";

        if (!_meshes.TryGetValue(name, out TrMesh? mesh))
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

            mesh = new(context, name, vertices, [.. vertices.Select((a, b) => (uint)b)]);

            _meshes.Add(name, mesh);
        }

        return mesh;
    }

    public static TrMesh GetPlane(this TrContext context, float size = 0.5f)
    {
        string name = $"Plane";

        if (!_meshes.TryGetValue(name, out TrMesh? mesh))
        {
            TrVertex[] vertices =
            [
                new(new(-size, 0.0f, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
                new(new(size, 0.0f, size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
                new(new(size, 0.0f, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 1.0f)),
                new(new(size, 0.0f, size), new(0.0f, 1.0f, 0.0f), texCoord: new(1.0f, 0.0f)),
                new(new(-size, 0.0f, -size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 1.0f)),
                new(new(-size, 0.0f, size), new(0.0f, 1.0f, 0.0f), texCoord: new(0.0f, 0.0f))
            ];

            mesh = new(context, name, vertices, [.. vertices.Select((a, b) => (uint)b)]);

            _meshes.Add(name, mesh);
        }

        return mesh;
    }

    public static (TrMesh[] Meshes, TrMaterialProperty[] MaterialProperties) AssimpParsing(this TrContext context, string file)
    {
        if (!_models.TryGetValue(file, out Tuple<TrMesh[], TrMaterialProperty[]>? tuple))
        {
            const PostProcessSteps steps = PostProcessSteps.CalculateTangentSpace
                                           | PostProcessSteps.Triangulate
                                           | PostProcessSteps.GenerateNormals
                                           | PostProcessSteps.GenerateSmoothNormals
                                           | PostProcessSteps.GenerateUVCoords
                                           | PostProcessSteps.OptimizeMeshes
                                           | PostProcessSteps.OptimizeGraph
                                           | PostProcessSteps.PreTransformVertices;

            using Assimp importer = Assimp.GetApi();
            Scene* scene = importer.ImportFile(file, (uint)steps);

            if (scene == null)
            {
                throw new TrException($"Assimp parsing failed. Error: {importer.GetErrorStringS()}");
            }

            List<TrMesh> tempMeshes = [];
            List<TrMaterialProperty> tempMaterialProperties = [];

            ProcessNode(scene->MRootNode);
            ProcessMaterials();

            tuple = new([.. tempMeshes], [.. tempMaterialProperties]);

            _models.Add(file, tuple);

            void ProcessNode(Node* node)
            {
                for (uint i = 0; i < node->MNumMeshes; i++)
                {
                    Mesh* mesh = scene->MMeshes[node->MMeshes[i]];

                    tempMeshes.Add(ProcessMesh(mesh));
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

                    if (mesh->MTangents != null)
                    {
                        vertices[i].Tangent = (*&mesh->MTangents[i]).ToGeneric();
                    }

                    if (mesh->MBitangents != null)
                    {
                        vertices[i].Bitangent = (*&mesh->MBitangents[i]).ToGeneric();
                    }

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

                return new(context, $"{file} - {mesh->MName.AsString}", vertices, indices, mesh->MMaterialIndex);
            }

            void ProcessMaterials()
            {
                for (uint i = 0; i < scene->MNumMaterials; i++)
                {
                    Material* material = scene->MMaterials[i];

                    tempMaterialProperties.Add(ProcessMaterial(material));
                }
            }

            TrMaterialProperty ProcessMaterial(Material* material)
            {
                for (uint i = 0; i < material->MNumProperties; i++)
                {

                }

                return new TrMaterialProperty(context);
            }
        }

        return (tuple.Item1, tuple.Item2);
    }
}
