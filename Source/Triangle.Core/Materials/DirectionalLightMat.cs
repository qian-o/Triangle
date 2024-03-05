﻿using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using AttribLocation = uint;

namespace Triangle.Core.Materials;

internal sealed class DirectionalLightMat(TrContext context) : TrMaterial(context, "DirectionalLight")
{
    public const AttribLocation InPosition = 0;

    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniTransforms
    {
        [FieldOffset(0)]
        public Matrix4X4<float> Model;

        [FieldOffset(64)]
        public Matrix4X4<float> View;

        [FieldOffset(128)]
        public Matrix4X4<float> Projection;

        [FieldOffset(192)]
        public Matrix4X4<float> ObjectToWorld;

        [FieldOffset(256)]
        public Matrix4X4<float> ObjectToClip;

        [FieldOffset(320)]
        public Matrix4X4<float> WorldToObject;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;
    }
    #endregion

    private TrBuffer<UniTransforms> uboTransforms = null!;
    private TrBuffer<UniParameters> uboParameters = null!;

    public override TrRenderPass CreateRenderPass()
    {
        uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/DirectionalLight/DirectionalLight.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/DirectionalLight/DirectionalLight.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);
        renderPipeline.IsCullFace = false;
        renderPipeline.Polygon = new(TrTriangleFace.FrontAndBack, TrPolygonMode.Line);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    /// <summary>
    /// Draw directional light mesh.
    /// </summary>
    /// <param name="mesh">mesh</param>
    /// <param name="args">
    /// args:
    /// args[0] - Transform matrix
    /// args[1] - Camera
    /// args[2] - Color vec3
    /// </param>
    public override void Draw(TrMesh mesh, params object[] args)
    {
        if (args.Length != 3 || args[0] is not Matrix4X4<float> model || args[1] is not TrCamera camera || args[2] is not Vector3D<float> color)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer(InPosition, 3, nameof(TrVertex.Position));

            uboTransforms.SetData(new UniTransforms()
            {
                Model = model,
                View = camera.View,
                Projection = camera.Projection,
                ObjectToWorld = model,
                ObjectToClip = model * camera.View * camera.Projection,
                WorldToObject = model.Invert()
            });
            uboParameters.SetData(new UniParameters()
            {
                Color = new(color.X, color.Y, color.Z, 1.0f)
            });

            renderPipeline.BindUniformBlock(0, uboTransforms);
            renderPipeline.BindUniformBlock(1, uboParameters);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void ControllerCore()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        RenderPass.Dispose();

        uboTransforms.Dispose();
        uboParameters.Dispose();
    }
}