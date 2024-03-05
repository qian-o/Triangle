﻿using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class PrefilterMat(TrContext context) : GlobalMat(context, "Prefilter")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Matrix4X4<float> View;

        [FieldOffset(64)]
        public Matrix4X4<float> Projection;

        [FieldOffset(128)]
        public float Roughness;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public Matrix4X4<float> View { get; set; }

    public Matrix4X4<float> Projection { get; set; }

    public float Roughness { get; set; }

    public override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Prefilter/Prefilter.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Prefilter/Prefilter.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh mesh, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters
        {
            View = View,
            Projection = Projection,
            Roughness = Roughness
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        mesh.Draw();

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        AdjustChannel("Sky Map", 0);
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}