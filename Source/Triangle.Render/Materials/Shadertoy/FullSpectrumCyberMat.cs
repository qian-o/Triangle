using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Shadertoy;

public class FullSpectrumCyberMat(TrContext context) : GlobalMat(context, "FullSpectrumCyber")
{
    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Shadertoy/FullSpectrumCyber/FullSpectrumCyber.vert.spv".PathFormatter());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Shadertoy/FullSpectrumCyber/FullSpectrumCyber.frag.spv".PathFormatter());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore([NotNull] TrMesh mesh, [NotNull] GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        mesh.Draw();

        renderPipeline.Unbind();
    }

    protected override void AdjustImGuiPropertiesCore()
    {
    }

    protected override void DestroyCore(bool disposing = false)
    {
    }
}
