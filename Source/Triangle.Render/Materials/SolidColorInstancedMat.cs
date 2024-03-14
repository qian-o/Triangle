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
    private readonly TrTexture _colorSampler = new(context);

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
            for (int i = 0; i < indices.Length; i++)
            {
                color[i] = Color[indices[i]];
            }
        }

        fixed (Vector4D<float>* dataPtr = &color[0])
        {
            _colorSampler.Write(1, (uint)color.Length, TrPixelFormat.RGBA16F, dataPtr);
        }
    }

    protected override void InstancedCore(TrMesh[] meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        renderPipeline.BindUniformBlock(UniformSamplerBindingStart + 0, _colorSampler);

        meshes.First().DrawInstanced(meshes.Length);

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
    }

    protected override void DestroyCore(bool disposing = false)
    {
        _colorSampler.Dispose();
    }
}
