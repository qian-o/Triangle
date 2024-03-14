﻿using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Shadertoy;
public class RaymarchingPrimitivesMat(TrContext context) : GlobalMat(context, "RaymarchingPrimitives")
{
    protected override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Shadertoy/RaymarchingPrimitives/RaymarchingPrimitives.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Shadertoy/RaymarchingPrimitives/RaymarchingPrimitives.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh[] meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        foreach (TrMesh mesh in meshes)
        {
            mesh.Draw();
        }

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
    }

    protected override void DestroyCore(bool disposing = false)
    {
    }
}
