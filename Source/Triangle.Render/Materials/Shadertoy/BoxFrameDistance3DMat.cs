using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Shadertoy;

public class BoxFrameDistance3DMat(TrContext context) : GlobalMat(context, "BoxFrameDistance3D")
{
    protected override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Shadertoy/BoxFrameDistance3D/BoxFrameDistance3D.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Shadertoy/BoxFrameDistance3D/BoxFrameDistance3D.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
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
    }

    protected override void DestroyCore(bool disposing = false)
    {
    }
}
