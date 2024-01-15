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

public class BlinnPhongMat(TrContext context) : TrMaterial<TrParameter>(context, "BlinnPhong")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniTransforms
    {
        [FieldOffset(0)]
        public Matrix4X4<float> ObjectToWorld;

        [FieldOffset(64)]
        public Matrix4X4<float> ObjectToClip;

        [FieldOffset(128)]
        public Matrix4X4<float> WorldToObject;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniVectors
    {
        [FieldOffset(0)]
        public Vector3D<float> CameraPosition;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniMaterial
    {
        [FieldOffset(0)]
        public Vector4D<float> Diffuse;

        [FieldOffset(16)]
        public Vector4D<float> Specular;

        [FieldOffset(32)]
        public float Gloss;
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
    private TrBuffer<UniTransforms> uboTransforms = null!;
    private TrBuffer<UniVectors> uboVectors = null!;
    private TrBuffer<UniMaterial> uboMaterial = null!;
    private TrBuffer<UniAmbientLight> uboAmbientLight = null!;
    private TrBuffer<UniDirectionalLight> uboDirectionalLight = null!;

    public Vector4D<float> Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public Vector4D<float> Specular { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public float Gloss { get; set; } = 20.0f;

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/Chapter6/BlinnPhong.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/Chapter6/BlinnPhong.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboVectors = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
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

            uboTransforms.SetData(new UniTransforms()
            {
                ObjectToWorld = parameter.Model,
                ObjectToClip = parameter.Model * parameter.Camera.View * parameter.Camera.Projection,
                WorldToObject = Matrix4X4.Transpose(parameter.Model.Invert())
            });
            uboVectors.SetData(new UniVectors()
            {
                CameraPosition = parameter.Camera.Position
            });
            uboMaterial.SetData(new UniMaterial()
            {
                Diffuse = Diffuse,
                Specular = Specular,
                Gloss = Gloss
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

            renderPipeline.BindUniformBlock(0, uboTransforms);
            renderPipeline.BindUniformBlock(1, uboVectors);
            renderPipeline.BindUniformBlock(2, uboMaterial);
            renderPipeline.BindUniformBlock(3, uboAmbientLight);
            renderPipeline.BindUniformBlock(4, uboDirectionalLight);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 diffuse = Diffuse.ToSystem();
        ImGui.ColorEdit4("Diffuse", ref diffuse);
        Diffuse = diffuse.ToGeneric();

        Vector4 specular = Specular.ToSystem();
        ImGui.ColorEdit4("Specular", ref specular);
        Specular = specular.ToGeneric();

        float gloss = Gloss;
        ImGui.DragFloat("Gloss", ref gloss, 0.1f, 8.0f, 256f);
        Gloss = gloss;
    }

    protected override void Destroy(bool disposing = false)
    {
        uboDirectionalLight.Dispose();
        uboAmbientLight.Dispose();
        uboMaterial.Dispose();
        uboVectors.Dispose();
        uboTransforms.Dispose();

        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
