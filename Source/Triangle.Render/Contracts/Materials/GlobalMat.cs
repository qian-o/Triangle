using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ImGuiNET;
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
    public const uint UniformBufferBindingStart = 6;
    public const uint UniformSampler2dBindingStart = 4;

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

    [StructLayout(LayoutKind.Explicit)]
    private struct UniTexScaleOffset
    {
        [FieldOffset(0)]
        public Vector4D<float> Channel0ST;

        [FieldOffset(16)]
        public Vector4D<float> Channel1ST;

        [FieldOffset(32)]
        public Vector4D<float> Channel2ST;

        [FieldOffset(48)]
        public Vector4D<float> Channel3ST;
    }
    #endregion

    private readonly TrBuffer<UniTransforms> _uboTransforms;
    private readonly TrBuffer<UniVectors> _uboVectors;
    private readonly TrBuffer<UniConstants> _uboConstants;
    private readonly TrBuffer<UniAmbientLight> _uboAmbientLight;
    private readonly TrBuffer<UniDirectionalLight> _uboDirectionalLight;
    private readonly TrBuffer<UniTexScaleOffset> _uboTexScaleOffset;
    private readonly Dictionary<int, (PropertyInfo Channel, PropertyInfo ChannelST)> _channelCache;

    protected GlobalMat([NotNull] TrContext context, [NotNull] string name) : base(context, name)
    {
        _uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboVectors = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboConstants = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboTexScaleOffset = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        _channelCache = [];
    }

    public TrTexture? Channel0 { get; set; }

    public TrTexture? Channel1 { get; set; }

    public TrTexture? Channel2 { get; set; }

    public TrTexture? Channel3 { get; set; }

    public Vector4D<float> Channel0ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel1ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel2ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel3ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public override void Draw([NotNull] TrMesh mesh, [NotNull] GlobalParameters parameters)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Tangent"), 3, nameof(TrVertex.Tangent));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Bitangent"), 3, nameof(TrVertex.Bitangent));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Color"), 4, nameof(TrVertex.Color));
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
            _uboTexScaleOffset.SetData(new UniTexScaleOffset()
            {
                Channel0ST = Channel0ST,
                Channel1ST = Channel1ST,
                Channel2ST = Channel2ST,
                Channel3ST = Channel3ST
            });

            renderPipeline.BindUniformBlock(0, _uboTransforms);
            renderPipeline.BindUniformBlock(1, _uboVectors);
            renderPipeline.BindUniformBlock(2, _uboConstants);
            renderPipeline.BindUniformBlock(3, _uboAmbientLight);
            renderPipeline.BindUniformBlock(4, _uboDirectionalLight);
            renderPipeline.BindUniformBlock(5, _uboTexScaleOffset);

            renderPipeline.BindUniformBlock(0, Channel0);
            renderPipeline.BindUniformBlock(1, Channel1);
            renderPipeline.BindUniformBlock(2, Channel2);
            renderPipeline.BindUniformBlock(3, Channel3);

            renderPipeline.Unbind();
        }

        DrawCore(mesh, parameters);
    }

    public void AdjustChannel(int index)
    {
        if (!_channelCache.TryGetValue(index, out (PropertyInfo Channel, PropertyInfo ChannelST) cache))
        {
            Type type = GetType();

            if (type.GetProperty($"Channel{index}") is PropertyInfo tex && type.GetProperty($"Channel{index}ST") is PropertyInfo st)
            {
                _channelCache.Add(index, cache = (tex, st));
            }
            else
            {
                throw new InvalidOperationException($"Channel{index} or Channel{index}ST not found.");
            }
        }

        string name = $"Channel {index}";
        TrTexture? channel = (TrTexture?)cache.Channel.GetValue(this);
        Vector4D<float> channelST = (Vector4D<float>)cache.ChannelST.GetValue(this)!;

        ImGui.PushID(name);

        ImGui.Text(name);

        ImGui.BeginGroup();
        {
            ImGui.Text("Tiling");
            Vector2 s = new(channelST.X, channelST.Y);
            ImGui.DragFloat2("##Tiling", ref s, 0.01f);

            ImGui.Text("Offset");
            Vector2 t = new(channelST.Z, channelST.W);
            ImGui.DragFloat2("##Offset", ref t, 0.01f);

            channelST = new(s.X, s.Y, t.X, t.Y);
        }
        ImGui.EndGroup();

        ImGui.SameLine();

        float imageSize = ImGui.GetItemRectSize().Y;
        TrTextureManager.TextureSelection(name, new Vector2D<float>(imageSize), ref channel);

        ImGui.PopID();

        cache.Channel.SetValue(this, channel);
        cache.ChannelST.SetValue(this, channelST);
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
