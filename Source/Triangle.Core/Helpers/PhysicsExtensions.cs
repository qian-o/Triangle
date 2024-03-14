using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Silk.NET.Maths;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using BepuTriangle = BepuPhysics.Collidables.Triangle;

namespace Triangle.Core.Helpers;

public static class PhysicsExtensions
{
    public static Mesh[] ToMesh(this TrModel model, BufferPool bufferPool, bool mergeMesh = false)
    {
        List<Mesh> meshes = [];

        if (mergeMesh)
        {
            List<BepuTriangle> triangles = [];

            foreach (TrMesh mesh in model.Meshes)
            {
                for (int i = 0; i < mesh.IndicesLength; i += 3)
                {
                    triangles.Add(new BepuTriangle
                    {
                        A = mesh.Vertices[(int)mesh.Indices[i]].Position.ToSystem(),
                        B = mesh.Vertices[(int)mesh.Indices[i + 1]].Position.ToSystem(),
                        C = mesh.Vertices[(int)mesh.Indices[i + 2]].Position.ToSystem()
                    });
                }
            }

            bufferPool.Take(triangles.Count, out Buffer<BepuTriangle> buffer);

            for (int i = 0; i < triangles.Count; i++)
            {
                buffer[i] = triangles[i];
            }

            meshes.Add(new Mesh(buffer, model.Transform.Scale.ToSystem(), bufferPool));
        }
        else
        {
            foreach (TrMesh mesh in model.Meshes)
            {
                List<BepuTriangle> triangles = [];

                for (int i = 0; i < mesh.IndicesLength; i += 3)
                {
                    triangles.Add(new BepuTriangle
                    {
                        A = mesh.Vertices[(int)mesh.Indices[i]].Position.ToSystem(),
                        B = mesh.Vertices[(int)mesh.Indices[i + 1]].Position.ToSystem(),
                        C = mesh.Vertices[(int)mesh.Indices[i + 2]].Position.ToSystem()
                    });
                }

                bufferPool.Take(triangles.Count, out Buffer<BepuTriangle> buffer);

                for (int i = 0; i < triangles.Count; i++)
                {
                    buffer[i] = triangles[i];
                }

                meshes.Add(new Mesh(buffer, model.Transform.Scale.ToSystem(), bufferPool));
            }
        }

        return [.. meshes];
    }
}
