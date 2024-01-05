using Common.Models;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials;

public unsafe class DiffuseVertexLevelMat(TrContext context) : TrMaterial<TrParameter>(context)
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniCamera
    {
        [FieldOffset(0)]
        public Matrix4X4<float> Model;

        [FieldOffset(64)]
        public Matrix4X4<float> View;

        [FieldOffset(128)]
        public Matrix4X4<float> Projection;
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
        public Vector3D<float> Direction;

        [FieldOffset(16)]
        public Vector3D<float> Color;
    }
    #endregion

    private TrRenderPipeline renderPipeline = null!;
    private TrBuffer<UniCamera> uboCamera = null!;
    private TrBuffer<UniMaterial> uboMaterial = null!;
    private TrBuffer<UniAmbientLight> uboAmbientLight = null!;
    private TrBuffer<UniDirectionalLight> uboDirectionalLight = null!;

    public Vector4D<float> Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/DiffuseVertexLevelShader.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/DiffuseVertexLevelShader.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        uboCamera = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboMaterial = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        GL gl = Context.GL;

        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            uboCamera.SetData(new UniCamera()
            {
                Model = parameter.Model,
                View = parameter.Camera.View,
                Projection = parameter.Camera.Projection
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
                Direction = parameter.DirectionalLight.Direction,
                Color = parameter.DirectionalLight.Color
            });

            renderPipeline.BindUniformBlock(0, uboCamera);
            renderPipeline.BindUniformBlock(1, uboMaterial);
            renderPipeline.BindUniformBlock(2, uboAmbientLight);
            renderPipeline.BindUniformBlock(3, uboDirectionalLight);

            gl.BindVertexArray(mesh.Handle);
            gl.DrawElements(GLEnum.Triangles, (uint)mesh.IndexLength, GLEnum.UnsignedInt, null);
            gl.BindVertexArray(0);

            renderPipeline.Unbind();
        }
    }

    public override void ImGuiEdit()
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
        uboCamera.Dispose();

        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
