﻿using Silk.NET.Assimp;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace Triangle.Render.Helpers;

public static unsafe class TrMeshFactory
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

    public static TrMesh[] AssimpParsing(this TrContext context, string file)
    {
        using Assimp importer = Assimp.GetApi();
        Scene* scene = importer.ImportFile(file, (uint)(PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals | PostProcessSteps.CalculateTangentSpace | PostProcessSteps.FlipUVs | PostProcessSteps.PreTransformVertices));

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

                if (mesh->MTextureCoords[0] != null)
                {
                    Vector3D<float> vector3D = (*&mesh->MTextureCoords[0][i]).ToGeneric();

                    vertices[i].TexCoord = new Vector2D<float>(vector3D.X, vector3D.Y);
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

            return new(context, vertices, indices);
        }
    }
}