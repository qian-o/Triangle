﻿using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class SolidColorMat(TrContext context) : GlobalMat(context, "SolidColor")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/SolidColor/SolidColor.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/SolidColor/SolidColor.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        uboParameters.SetData(new UniParameters()
        {
            Color = Color
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
        Vector4D<float> color = Color;
        ImGuiHelper.ColorEdit4("Color", ref color);
        Color = color;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
