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
        public float Exposure;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public float Exposure { get; set; } = 1.0f;

    public override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        Channel0 = TrTextureManager.Texture("Resources/Textures/Skies/cloudy_puresky_4k.hdr".Path());

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Sky/Sky.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Sky/Sky.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Background);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh mesh, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters()
        {
            Exposure = Exposure
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        mesh.Draw();

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        float exposure = Exposure;
        ImGuiHelper.SliderFloat("Exposure", ref exposure, 0.1f, 10.0f);
        Exposure = exposure;

        AdjustChannel("Sky Tex", 0);
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
