using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Materials;

public unsafe abstract class GlobalInstancedMat(TrContext context, string name) : GlobalMat(context, name)
{
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
    public override void Draw(IEnumerable<TrModel> models, params object[] args)
    {
        if (args.FirstOrDefault(item => item is GlobalParameters) is not GlobalParameters parameters)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        TrModel[] arr = [.. models];

        int pages = (int)Math.Ceiling((double)arr.Length / 4096);

        for (int i = 0; i < pages; i++)
        {
            IEnumerable<TrModel> page = arr.Skip(i * 4096).Take(4096);

            InternalDraw(page, i * 4096, page.Count());
        }

        void InternalDraw(IEnumerable<TrModel> models, int skip, int length)
        {
            UpdateMatrixSampler(models.Select(item => item.Transform.Model).ToArray());

            UpdateSampler(skip, length);

            foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
            {
                renderPipeline.Bind();

                renderPipeline.BindUniformBlock(10, _matrixSampler);

                renderPipeline.Unbind();
            }

            string[] names = models.SelectMany(item => item.Meshes).Select(item => item.Name).Distinct().ToArray();

            foreach (string name in names)
            {
                // TODO: 应该按照 Mesh 进行分组，然后更新 Sampler 再去绘制。
            }
        }
    }

    protected override void DrawCore(IEnumerable<TrMesh> meshes, GlobalParameters globalParameters)
    {
        InstancedCore(meshes, globalParameters);
    }

    protected abstract void UpdateSampler(int skip, int length);

    protected abstract void InstancedCore(IEnumerable<TrMesh> meshes, GlobalParameters globalParameters);

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
