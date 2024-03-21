using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public unsafe class SolidColorInstancedMat(TrContext context) : GlobalInstancedMat(context, "SolidColorInstanced")
{
    private readonly TrPixelBuffer _colorSampler = new(context, 1, MaxSamplerSize, TrPixelFormat.RGBA16F);

    public Vector4D<float>[]? Color { get; set; }

    protected override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/SolidColor/SolidColorInstanced.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/SolidColor/SolidColorInstanced.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void UpdateSampler(int[] indices)
    {
        Vector4D<float>[] color = new Vector4D<float>[indices.Length];

        if (Color != null)
        {
            if (Color.Length == 1)
            {
                Array.Fill(color, Color[0]);
            }
            else
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    color[i] = Color[indices[i]];
                }
            }
        }

        _colorSampler.Update(1, color.Length, color);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        base.AssemblePipeline(renderPipeline, globalParameters);

        renderPipeline.BindUniformBlock(UniformSamplerBindingStart + 0, _colorSampler);
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
        _colorSampler.Dispose();
    }
}
