using System.Runtime.InteropServices;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class SkyMat(TrContext context) : GlobalMat(context, "Sky")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public bool GammaCorrection;

        [FieldOffset(4)]
        public float Gamma;

        [FieldOffset(8)]
        public float Exposure;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public bool GammaCorrection { get; set; }

    public float Gamma { get; set; } = 2.2f;

    public float Exposure { get; set; } = 1.0f;

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        Channel0 = TrTextureManager.Texture("Resources/Textures/Skies/kloppenheim_06_puresky_4k/kloppenheim_06_puresky_4k.hdr".Path());

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Sky/Sky.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Sky/Sky.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Background);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        uboParameters.SetData(new UniParameters()
        {
            GammaCorrection = GammaCorrection,
            Gamma = Gamma,
            Exposure = Exposure
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);
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
        bool gammaCorrection = GammaCorrection;
        ImGuiHelper.Checkbox("Gamma Correction", ref gammaCorrection);
        GammaCorrection = gammaCorrection;

        if (GammaCorrection)
        {
            float gamma = Gamma;
            ImGuiHelper.DragFloat("Gamma", ref gamma, 0.01f, 0.1f, 3.0f);
            Gamma = gamma;

            float exposure = Exposure;
            ImGuiHelper.DragFloat("Exposure", ref exposure, 0.01f, 0.1f, 10.0f);
            Exposure = exposure;
        }

        AdjustChannel("Sky Tex", 0, "Resources/Textures/Skies".Path());
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
