using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class PBRMat(TrContext context) : GlobalMat(context, "PBR")
{
    public override TrRenderPass CreateRenderPass()
    {
        Channel0 = TrTextureManager.Texture("Resources/Textures/Rusted Iron/Albedo.png".Path());
        Channel1 = TrTextureManager.Texture("Resources/Textures/Rusted Iron/Normal.png".Path());
        Channel2 = TrTextureManager.Texture("Resources/Textures/Rusted Iron/Metallic.png".Path());
        Channel3 = TrTextureManager.Texture("Resources/Textures/Rusted Iron/Roughness.png".Path());
        Channel4 = TrTextureManager.Texture("Resources/Textures/Rusted Iron/AmbientOcclusion.png".Path());

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/PBR/PBR.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/PBR/PBR.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh mesh, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];
        renderPipeline.Bind();

        mesh.Draw();

        renderPipeline.Unbind();
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
    }
}
