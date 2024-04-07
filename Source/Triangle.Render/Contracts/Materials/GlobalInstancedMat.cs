using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Materials;

public unsafe abstract class GlobalInstancedMat(TrContext context, string name) : GlobalMat(context, name)
{
    public const uint BufferBindingStart = 1;
    public const int MaxSamplerSize = 1024;

    private readonly TrBuffer<Matrix4X4<float>> _bufferMatrix = new(context, MaxSamplerSize);

    /// <summary>
    /// Draw the model with the material.
    /// </summary>
    /// <param name="models">models</param>
    /// <param name="args">
    /// args:
    /// GlobalParameters parameters - Global parameters
    /// </param>
    /// <exception cref="ArgumentException">
    /// If `GlobalParameters` is not found in the args.
    /// </exception>
    public override void Draw(TrModel[] models, params object[] args)
    {
        if (args.FirstOrDefault(item => item is GlobalParameters) is not GlobalParameters parameters)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        int pages = (int)Math.Ceiling((double)models.Length / MaxSamplerSize);

        if (pages > 1)
        {
            Matrix4X4<float>[] transforms = new Matrix4X4<float>[models.Length];
            Parallel.ForEach(Partitioner.Create(0, models.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    transforms[i] = models[i].Transform.Model;
                }
            });

            for (int i = 0; i < pages; i++)
            {
                InternalDraw([.. models.Skip(i * MaxSamplerSize).Take(MaxSamplerSize)], [.. transforms.Skip(i * MaxSamplerSize).Take(MaxSamplerSize)], i * MaxSamplerSize, parameters);
            }
        }
        else
        {
            InternalDraw(models, [.. models.Select(item => item.Transform.Model)], 0, parameters);
        }
    }

    public void Draw(TrMesh mesh, Matrix4X4<float>[] transforms, params object[] args)
    {
        if (args.FirstOrDefault(item => item is GlobalParameters) is not GlobalParameters parameters)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        int length = transforms.Length;

        TrMesh[] meshes = new TrMesh[length];
        Array.Fill(meshes, mesh);

        int[] indices = Enumerable.Range(0, length).ToArray();

        int pages = (int)Math.Ceiling((double)length / MaxSamplerSize);

        if (pages > 1)
        {
            for (int i = 0; i < pages; i++)
            {
                int skip = i * MaxSamplerSize;

                UpdateMatrixSampler([.. transforms.Skip(skip).Take(MaxSamplerSize)]);
                UpdateSampler([.. indices.Skip(skip).Take(MaxSamplerSize)]);

                Draw([.. meshes.Skip(skip).Take(MaxSamplerSize)], parameters);
            }
        }
        else
        {
            UpdateMatrixSampler(transforms);
            UpdateSampler(indices);

            Draw(meshes, parameters);
        }
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        renderPipeline.BindBufferBlock(0, _bufferMatrix);
    }

    protected abstract void UpdateSampler(int[] indices);

    protected override void Destroy(bool disposing = false)
    {
        _bufferMatrix.Dispose();

        base.Destroy(disposing);
    }

    private void UpdateMatrixSampler(Matrix4X4<float>[] models)
    {
        _bufferMatrix.SetData(models);
    }

    private void InternalDraw(TrModel[] models, Matrix4X4<float>[] transforms, int beginIndex, GlobalParameters parameters)
    {
        (TrModel Model, ReadOnlyCollection<TrMesh> Meshes)[] map = [.. models.Select(item => (item, item.Meshes))];

        foreach (string name in map.SelectMany(item => item.Meshes).Select(item => item.Name).Distinct())
        {
            List<TrModel> drawModels = new(map.Length);
            List<Matrix4X4<float>> drawTransforms = new(map.Length);
            List<int> indices = new(map.Length);
            List<TrMesh> meshes = new(map.Length);

            for (int i = 0; i < map.Length; i++)
            {
                if (map[i].Meshes.FirstOrDefault(item => item.Name == name) is TrMesh mesh)
                {
                    drawModels.Add(models[i]);
                    drawTransforms.Add(transforms[i]);
                    indices.Add(beginIndex + i);
                    meshes.Add(mesh);
                }
            }

            UpdateMatrixSampler([.. drawTransforms]);
            UpdateSampler([.. indices]);

            Draw([.. meshes], parameters);
        }
    }
}
