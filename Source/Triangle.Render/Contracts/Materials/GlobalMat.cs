using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Materials;

public abstract class GlobalMat : TrMaterial<GlobalParameters>
{
    public const uint UniformBufferBindingStart = 5;

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
        public Vector2D<float> Resolution;

        [FieldOffset(16)]
        public Vector3D<float> CameraPosition;

        [FieldOffset(32)]
        public Vector3D<float> CameraUp;

        [FieldOffset(48)]
        public Vector3D<float> CameraRight;

        [FieldOffset(64)]
        public Vector4D<float> Mouse;

        [FieldOffset(80)]
        public Vector4D<float> Date;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniConstants
    {
        [FieldOffset(0)]
        public float Time;

        [FieldOffset(4)]
        public float DeltaTime;

        [FieldOffset(8)]
        public float FrameRate;

        [FieldOffset(12)]
        public int Frame;
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
    private readonly TrBuffer<UniConstants> _uboConstants;
    private readonly TrBuffer<UniAmbientLight> _uboAmbientLight;
    private readonly TrBuffer<UniDirectionalLight> _uboDirectionalLight;

    protected GlobalMat([NotNull] TrContext context, [NotNull] string name) : base(context, name)
    {
        _uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboVectors = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboConstants = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] GlobalParameters parameters)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            _uboTransforms.SetData(new UniTransforms()
            {
                Model = parameters.Model,
                View = parameters.Camera.View,
                Projection = parameters.Camera.Projection,
                ObjectToWorld = parameters.Model,
                ObjectToClip = parameters.Model * parameters.Camera.View * parameters.Camera.Projection,
                WorldToObject = Matrix4X4.Transpose(parameters.Model.Invert())
            });
            _uboVectors.SetData(new UniVectors()
            {
                Resolution = parameters.SceneData.Resolution,
                CameraPosition = parameters.Camera.Position,
                CameraUp = parameters.Camera.Up,
                CameraRight = parameters.Camera.Right,
                Mouse = parameters.SceneData.Mouse,
                Date = parameters.SceneData.Date
            });
            _uboConstants.SetData(new UniConstants()
            {
                Time = parameters.SceneData.Time,
                DeltaTime = parameters.SceneData.DeltaTime,
                FrameRate = parameters.SceneData.FrameRate,
                Frame = parameters.SceneData.Frame
            });
            _uboAmbientLight.SetData(new UniAmbientLight()
            {
                Color = parameters.AmbientLight.Color
            });
            _uboDirectionalLight.SetData(new UniDirectionalLight()
            {
                Color = parameters.DirectionalLight.Color,
                Position = -parameters.DirectionalLight.Direction,
                Direction = parameters.DirectionalLight.Direction,
            });

            renderPipeline.BindUniformBlock(0, _uboTransforms);
            renderPipeline.BindUniformBlock(1, _uboVectors);
            renderPipeline.BindUniformBlock(2, _uboConstants);
            renderPipeline.BindUniformBlock(3, _uboAmbientLight);
            renderPipeline.BindUniformBlock(4, _uboDirectionalLight);

            renderPipeline.Unbind();
        }

        DrawCore(mesh, parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        _uboTransforms.Dispose();
        _uboVectors.Dispose();
        _uboConstants.Dispose();
        _uboAmbientLight.Dispose();
        _uboDirectionalLight.Dispose();

        DestroyCore(disposing);

        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Dispose();
        }
        RenderPass.Dispose();
    }

    protected abstract void DrawCore([NotNull] TrMesh mesh, [NotNull] GlobalParameters globalParameters);

    /// <summary>
    /// 此处应该清理材质中用到的其他缓冲区资源。
    /// 不要在该函数中清理渲染管道和管线。
    /// </summary>
    /// <param name="disposing"></param>
    protected abstract void DestroyCore(bool disposing = false);
}
