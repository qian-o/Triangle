using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Materials;

public unsafe abstract class GlobalInstancedMat(TrContext context, string name) : GlobalMat(context, name)
{
    public const int MaxSamplerSize = 4096;

    private readonly TrTexture _matrixSampler = new(context);

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

        TrModel[] arr = [.. models];

        int pages = (int)Math.Ceiling((double)arr.Length / MaxSamplerSize);

        for (int i = 0; i < pages; i++)
        {
            IEnumerable<TrModel> page = arr.Skip(i * MaxSamplerSize).Take(MaxSamplerSize);

            InternalDraw(page.ToArray(), i * 4096);
        }

        void InternalDraw(TrModel[] models, int beginIndex)
        {
            foreach (string name in models.SelectMany(item => item.Meshes).Select(item => item.Name).Distinct())
            {
                List<TrModel> drawModels = [];
                List<int> indices = [];
                List<TrMesh> meshes = [];

                for (int i = 0; i < models.Length; i++)
                {
                    if (models[i].Meshes.FirstOrDefault(item => item.Name == name) is TrMesh mesh)
                    {
                        drawModels.Add(models[i]);
                        indices.Add(beginIndex + i);
                        meshes.Add(mesh);
                    }
                }

                UpdateMatrixSampler(drawModels.Select(item => item.Transform.Model).ToArray());
                UpdateSampler([.. indices]);

                foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
                {
                    renderPipeline.Bind();

                    renderPipeline.BindUniformBlock(10, _matrixSampler);

                    renderPipeline.Unbind();
                }

                Draw([.. meshes], parameters);
            }
        }
    }

    protected override void DrawCore(TrMesh[] meshes, GlobalParameters globalParameters)
    {
        InstancedCore(meshes, globalParameters);
    }

    protected abstract void UpdateSampler(int[] indices);

    protected abstract void InstancedCore(TrMesh[] meshes, GlobalParameters globalParameters);

    protected override void Destroy(bool disposing = false)
    {
        _matrixSampler.Dispose();

        base.Destroy(disposing);
    }

    private void UpdateMatrixSampler(Matrix4X4<float>[] models)
    {
        fixed (Matrix4X4<float>* dataPtr = &models[0])
        {
            _matrixSampler.Write(4, (uint)models.Length, TrPixelFormat.RGBA16F, dataPtr);
        }
    }
}
