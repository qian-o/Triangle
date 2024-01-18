using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.App.Models;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;

namespace Triangle.App.Contracts.Materials;

public abstract class GlobalMat : TrMaterial<TrSceneParameters>
{
    public const uint UniformBufferBindingStart = 4;

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
    private struct UniVectors
    {
        [FieldOffset(0)]
        public Vector3D<float> CameraPosition;

        [FieldOffset(16)]
        public Vector3D<float> CameraUp;

        [FieldOffset(32)]
        public Vector3D<float> CameraRight;
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
        public Vector3D<float> Color;

        [FieldOffset(16)]
        public Vector3D<float> Position;

        [FieldOffset(32)]
        public Vector3D<float> Direction;
    }
    #endregion

    private readonly TrBuffer<UniTransforms> _uboTransforms;
    private readonly TrBuffer<UniVectors> _uboVectors;
    private readonly TrBuffer<UniAmbientLight> _uboAmbientLight;
    private readonly TrBuffer<UniDirectionalLight> _uboDirectionalLight;

    protected GlobalMat([NotNull] TrContext context, [NotNull] string name) : base(context, name)
    {
        _uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboVectors = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrSceneParameters parameter)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            _uboTransforms.SetData(new UniTransforms()
            {
                Model = parameter.Model,
                View = parameter.Camera.View,
                Projection = parameter.Camera.Projection,
                ObjectToWorld = parameter.Model,
                ObjectToClip = parameter.Model * parameter.Camera.View * parameter.Camera.Projection,
                WorldToObject = Matrix4X4.Transpose(parameter.Model.Invert())
            });
            _uboVectors.SetData(new UniVectors()
            {
                CameraPosition = parameter.Camera.Position,
                CameraUp = parameter.Camera.Up,
                CameraRight = parameter.Camera.Right
            });
            _uboAmbientLight.SetData(new UniAmbientLight()
            {
                Color = parameter.AmbientLight.Color
            });
            _uboDirectionalLight.SetData(new UniDirectionalLight()
            {
                Color = parameter.DirectionalLight.Color,
                Position = -parameter.DirectionalLight.Direction,
                Direction = parameter.DirectionalLight.Direction,
            });

            renderPipeline.BindUniformBlock(0, _uboTransforms);
            renderPipeline.BindUniformBlock(1, _uboVectors);
            renderPipeline.BindUniformBlock(2, _uboAmbientLight);
            renderPipeline.BindUniformBlock(3, _uboDirectionalLight);

            renderPipeline.Unbind();
        }

        DrawCore(mesh, parameter);
    }

    protected override void Destroy(bool disposing = false)
    {
        _uboTransforms.Dispose();
        _uboVectors.Dispose();
        _uboAmbientLight.Dispose();
        _uboDirectionalLight.Dispose();

        DestroyCore(disposing);

        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Dispose();
        }
        RenderPass.Dispose();
    }

    protected abstract void DrawCore([NotNull] TrMesh mesh, [NotNull] TrSceneParameters sceneParameters);

    /// <summary>
    /// 此处应该清理材质中用到的其他缓冲区资源。
    /// 不要在该函数中清理渲染管道和管线。
    /// </summary>
    /// <param name="disposing"></param>
    protected abstract void DestroyCore(bool disposing = false);
}
