using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Chapter6;

public unsafe class DiffusePixelLevelInstancedMat(TrContext context) : GlobalInstancedMat(context, "DiffusePixelLevelInstanced")
{
    private readonly TrTexture _diffuseSampler = new(context);

    public Vector4D<float>[]? Diffuse { get; set; }

    protected override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter6/DiffusePixelLevel/DiffusePixelLevelInstanced.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter6/DiffusePixelLevel/DiffusePixelLevelInstanced.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void UpdateSampler(int[] indices)
    {
        Vector4D<float>[] diffuse = new Vector4D<float>[indices.Length];

        if (Diffuse != null)
        {
            if (Diffuse.Length == 1)
            {
                Array.Fill(diffuse, Diffuse[0]);
            }
            else
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    diffuse[i] = Diffuse[indices[i]];
                }
            }
        }

        fixed (Vector4D<float>* dataPtr = &diffuse[0])
        {
            _diffuseSampler.Write(1, (uint)diffuse.Length, TrPixelFormat.RGBA16F, dataPtr);
        }
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        base.AssemblePipeline(renderPipeline, globalParameters);

        renderPipeline.BindUniformBlock(UniformSamplerBindingStart + 0, _diffuseSampler);
    }

    protected override void RenderPipeline(TrRenderPipeline renderPipeline, TrMesh[] meshes, GlobalParameters globalParameters)
    {
        meshes.First().DrawInstanced(meshes.Length);
    }

    protected override void ControllerCore()
    {
    }

    protected override void DestroyCore(bool disposing = false)
    {
        _diffuseSampler.Dispose();
    }
}
