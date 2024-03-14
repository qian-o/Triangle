using System.Runtime.InteropServices;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class PBRMat(TrContext context) : GlobalMat(context, "PBR")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public int MaxMipLevels;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public int MaxMipLevels { get; set; }

    public TrTexture? BRDF { get; set; }

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/PBR/PBR.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/PBR/PBR.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        uboParameters.SetData(new UniParameters
        {
            MaxMipLevels = MaxMipLevels
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        renderPipeline.BindUniformBlock(UniformSamplerBindingStart + 0, BRDF);
    }

    protected override void RenderPipeline(TrRenderPipeline renderPipeline, TrMesh[] meshes, GlobalParameters globalParameters)
    {
        foreach (TrMesh mesh in meshes)
        {
            mesh.Draw();
        }
    }

    protected override void ControllerCore()
    {
        AdjustChannel("Albedo", 0);
        AdjustChannel("Normal", 1);
        AdjustChannel("Metallic", 2);
        AdjustChannel("Roughness", 3);
        AdjustChannel("AmbientOcclusion", 4);
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
