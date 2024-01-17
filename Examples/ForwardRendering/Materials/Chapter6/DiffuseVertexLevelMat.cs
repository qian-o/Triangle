using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Common.Models;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials.Chapter6;

public class DiffuseVertexLevelMat(TrContext context) : TrMaterial<TrParameter>(context, "DiffuseVertexLevel")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMatrix
    {
        [FieldOffset(0)]
        public Matrix4X4<float> ObjectToWorld;

        [FieldOffset(64)]
        public Matrix4X4<float> ObjectToClip;

        [FieldOffset(128)]
        public Matrix4X4<float> WorldToObject;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniMaterial
    {
        [FieldOffset(0)]
        public Vector4D<float> Diffuse;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniAmbientLight
    {
        [FieldOffset(0)]
        public Vector3D<float> Color;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniDirectionalLight
    {
        [FieldOffset(0)]
        public Vector3D<float> Position;

        [FieldOffset(16)]
        public Vector3D<float> Direction;

        [FieldOffset(32)]
        public Vector3D<float> Color;
    }
    #endregion

    private TrRenderPipeline renderPipeline = null!;
    private TrBuffer<UniMatrix> uboMatrix = null!;
    private TrBuffer<UniMaterial> uboMaterial = null!;
    private TrBuffer<UniAmbientLight> uboAmbientLight = null!;
    private TrBuffer<UniDirectionalLight> uboDirectionalLight = null!;

    public Vector4D<float> Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter6/DiffuseVertexLevel.vert.spv");
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter6/DiffuseVertexLevel.frag.spv");

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        uboMatrix = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboMaterial = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            uboMatrix.SetData(new UniMatrix()
            {
                ObjectToWorld = parameter.Model,
                ObjectToClip = parameter.Model * parameter.Camera.View * parameter.Camera.Projection,
                WorldToObject = Matrix4X4.Transpose(parameter.Model.Invert())
            });
            uboMaterial.SetData(new UniMaterial()
            {
                Diffuse = Diffuse
            });
            uboAmbientLight.SetData(new UniAmbientLight()
            {
                Color = parameter.AmbientLight.Color
            });
            uboDirectionalLight.SetData(new UniDirectionalLight()
            {
                Position = -parameter.DirectionalLight.Direction,
                Direction = parameter.DirectionalLight.Direction,
                Color = parameter.DirectionalLight.Color
            });

            renderPipeline.BindUniformBlock(0, uboMatrix);
            renderPipeline.BindUniformBlock(1, uboMaterial);
            renderPipeline.BindUniformBlock(2, uboAmbientLight);
            renderPipeline.BindUniformBlock(3, uboDirectionalLight);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 diffuse = Diffuse.ToSystem();
        ImGui.ColorEdit4("Diffuse", ref diffuse);
        Diffuse = diffuse.ToGeneric();
    }

    protected override void Destroy(bool disposing = false)
    {
        uboDirectionalLight.Dispose();
        uboAmbientLight.Dispose();
        uboMaterial.Dispose();
        uboMatrix.Dispose();

        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
