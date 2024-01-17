using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Common.Models;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials;

public class GridMat(TrContext context) : TrMaterial<TrParameter>(context, "Grid")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMatrices
    {
        [FieldOffset(0)]
        public Matrix4X4<float> View;

        [FieldOffset(64)]
        public Matrix4X4<float> Projection;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public float Near;

        [FieldOffset(4)]
        public float Far;

        [FieldOffset(8)]
        public float PrimaryScale;

        [FieldOffset(12)]
        public float SecondaryScale;

        [FieldOffset(16)]
        public float GridIntensity;

        [FieldOffset(20)]
        public float Fade;
    }
    #endregion

    private TrRenderPipeline renderPipeline = null!;
    private TrBuffer<UniMatrices> uboMatrices = null!;
    private TrBuffer<UniParameters> uboParameters = null!;

    public float Distance { get; set; } = 6.0f;

    public float GridIntensity { get; set; } = 1.0f;

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Grid.vert.spv");
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Grid.frag.spv");

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        uboMatrices = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        double logDistance = Math.Log2(Distance);
        double upperDistance = Math.Pow(2.0, Math.Floor(logDistance) + 1);
        double lowerDistance = Math.Pow(2.0, Math.Floor(logDistance));
        float fade = Convert.ToSingle((Distance - lowerDistance) / (upperDistance - lowerDistance));

        double level = -Math.Floor(logDistance);
        float primaryScale = Convert.ToSingle(Math.Pow(2.0, level));
        float secondaryScale = Convert.ToSingle(Math.Pow(2.0, level + 1));

        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));

            uboMatrices.SetData(new UniMatrices()
            {
                View = parameter.Camera.View,
                Projection = parameter.Camera.Projection
            });

            uboParameters.SetData(new UniParameters()
            {
                Near = parameter.Camera.Near,
                Far = parameter.Camera.Far,
                PrimaryScale = primaryScale,
                SecondaryScale = secondaryScale,
                GridIntensity = GridIntensity,
                Fade = fade
            });

            renderPipeline.BindUniformBlock(0, uboMatrices);
            renderPipeline.BindUniformBlock(1, uboParameters);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        float distance = Distance;
        ImGui.SliderFloat("Distance", ref distance, 0.0f, 10.0f);
        Distance = distance;

        float gridIntensity = GridIntensity;
        ImGui.SliderFloat("Grid Intensity", ref gridIntensity, 0.0f, 1.0f);
        GridIntensity = gridIntensity;
    }

    protected override void Destroy(bool disposing = false)
    {
        uboParameters.Dispose();
        uboMatrices.Dispose();

        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
