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
            for (int i = 0; i < pages; i++)
            {
                InternalDraw([.. models.Skip(i * MaxSamplerSize).Take(MaxSamplerSize)], i * MaxSamplerSize, parameters);
            }
        }
        else
        {
            InternalDraw(models, 0, parameters);
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

    private void InternalDraw(TrModel[] models, int beginIndex, GlobalParameters parameters)
    {
        (TrModel Model, ReadOnlyCollection<TrMesh> Meshes)[] map = [.. models.Select(item => (item, item.Meshes))];

        foreach (string name in map.SelectMany(item => item.Meshes).Select(item => item.Name).Distinct())
        {
            List<TrModel> drawModels = new(map.Length);
            List<int> indices = new(map.Length);
            List<TrMesh> meshes = new(map.Length);

            for (int i = 0; i < map.Length; i++)
            {
                if (map[i].Meshes.FirstOrDefault(item => item.Name == name) is TrMesh mesh)
                {
                    drawModels.Add(models[i]);
                    indices.Add(beginIndex + i);
                    meshes.Add(mesh);
                }
            }

            UpdateMatrixSampler([.. drawModels.Select(item => item.Transform.Model)]);
            UpdateSampler([.. indices]);

            Draw([.. meshes], parameters);
        }
    }
}
