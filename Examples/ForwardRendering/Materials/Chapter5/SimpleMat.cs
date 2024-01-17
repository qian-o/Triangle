using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Common.Models;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials.Chapter5;

public class SimpleMat(TrContext context) : TrMaterial<TrParameter>(context, "Simple")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMatrices
    {
        [FieldOffset(0)]
        public Matrix4X4<float> Model;

        [FieldOffset(64)]
        public Matrix4X4<float> View;

        [FieldOffset(128)]
        public Matrix4X4<float> Projection;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;
    }
    #endregion

    private TrRenderPipeline renderPipeline = null!;
    private TrBuffer<UniMatrices> uboMatrices = null!;
    private TrBuffer<UniParameters> uboParameters = null!;

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter5/Simple.vert.spv");
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter5/Simple.frag.spv");

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        uboMatrices = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));

            uboMatrices.SetData(new UniMatrices()
            {
                Model = parameter.Model,
                View = parameter.Camera.View,
                Projection = parameter.Camera.Projection
            });

            uboParameters.SetData(new UniParameters()
            {
                Color = Color
            });

            renderPipeline.BindUniformBlock(0, uboMatrices);
            renderPipeline.BindUniformBlock(1, uboParameters);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 color = Color.ToSystem();
        ImGui.ColorEdit4("Color", ref color);
        Color = color.ToGeneric();
    }

    protected override void Destroy(bool disposing = false)
    {
        uboParameters.Dispose();
        uboMatrices.Dispose();

        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
