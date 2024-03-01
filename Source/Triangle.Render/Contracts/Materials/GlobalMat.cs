using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using Triangle.Render.Models;
using AttribLocation = uint;

namespace Triangle.Render.Contracts.Materials;

public abstract class GlobalMat : TrMaterial
{
    public const uint UniformBufferBindingStart = 8;
    public const uint UniformSampler2dBindingStart = 5;
    public const uint MaxPointLights = 50;

    public const AttribLocation InPosition = 0;
    public const AttribLocation InNormal = 1;
    public const AttribLocation InTangent = 2;
    public const AttribLocation InBitangent = 3;
    public const AttribLocation InColor = 4;
    public const AttribLocation InTexCoord = 5;

    #region Structs
    [StructLayout(LayoutKind.Explicit)]
    private struct PointLight
    {
        [FieldOffset(0)]
        public Vector3D<float> Color;

        [FieldOffset(16)]
        public Vector3D<float> Position;

        [FieldOffset(28)]
        public float Intensity;

        [FieldOffset(32)]
        public float Range;

        [FieldOffset(44)]
        public float _padding;
    }

    [InlineArray((int)MaxPointLights)]
    private struct PointLights
    {
        public PointLight _;
    }
    #endregion

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
        public int FrameCount;
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
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniPointLights
    {
        [FieldOffset(0)]
        public int Count;

        [FieldOffset(16)]
        public PointLights Lights;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniTexTexParams
    {
        [FieldOffset(0)]
        public Vector4D<float> Channel0Size;

        [FieldOffset(16)]
        public Vector4D<float> Channel1Size;

        [FieldOffset(32)]
        public Vector4D<float> Channel2Size;

        [FieldOffset(48)]
        public Vector4D<float> Channel3Size;

        [FieldOffset(64)]
        public Vector4D<float> Channel4Size;
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

        [FieldOffset(64)]
        public Vector4D<float> Channel4ST;
    }
    #endregion

    private readonly TrBuffer<UniTransforms> _uboTransforms;
    private readonly TrBuffer<UniVectors> _uboVectors;
    private readonly TrBuffer<UniConstants> _uboConstants;
    private readonly TrBuffer<UniAmbientLight> _uboAmbientLight;
    private readonly TrBuffer<UniDirectionalLight> _uboDirectionalLight;
    private readonly TrBuffer<UniPointLights> _uboPointLights;
    private readonly TrBuffer<UniTexTexParams> _uboUniTexTexParams;
    private readonly TrBuffer<UniTexScaleOffset> _uboTexScaleOffset;
    private readonly Dictionary<int, (PropertyInfo Channel, PropertyInfo ChannelST)> _channelCache;

    protected GlobalMat(TrContext context, string name) : base(context, name)
    {
        _uboTransforms = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboVectors = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboConstants = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboAmbientLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboDirectionalLight = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboPointLights = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboTexScaleOffset = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);
        _uboUniTexTexParams = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        _channelCache = [];
    }

    public TrTexture? Channel0 { get; set; }

    public TrTexture? Channel1 { get; set; }

    public TrTexture? Channel2 { get; set; }

    public TrTexture? Channel3 { get; set; }

    public TrTexture? Channel4 { get; set; }

    public Vector4D<float> Channel0ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel1ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel2ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel3ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public Vector4D<float> Channel4ST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    /// <summary>
    /// Draw the mesh with the material.
    /// </summary>
    /// <param name="mesh">mesh</param>
    /// <param name="args">
    /// args:
    /// Matrix4X4<float> model - Model matrix (can be null)
    /// GlobalParameters parameters - Global parameters
    /// </param>
    /// <exception cref="ArgumentException">
    /// If `GlobalParameters` is not found in the args.
    /// </exception>
    public override void Draw(TrMesh mesh, params object[] args)
    {
        if (args.FirstOrDefault(item => item is Matrix4X4<float>) is not Matrix4X4<float> model)
        {
            model = Matrix4X4<float>.Identity;
        }

        if (args.FirstOrDefault(item => item is GlobalParameters) is not GlobalParameters parameters)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        if (parameters.PointLights.Length > MaxPointLights)
        {
            throw new InvalidOperationException($"The maximum number of point lights is {MaxPointLights}.");
        }

        Vector4D<float> channel0Size = Vector4D<float>.Zero;
        Vector4D<float> channel1Size = Vector4D<float>.Zero;
        Vector4D<float> channel2Size = Vector4D<float>.Zero;
        Vector4D<float> channel3Size = Vector4D<float>.Zero;
        Vector4D<float> channel4Size = Vector4D<float>.Zero;

        if (Channel0 != null)
        {
            channel0Size = new Vector4D<float>(1.0f / Channel0.Width, 1.0f / Channel0.Height, Channel0.Width, Channel0.Height);
        }
        if (Channel1 != null)
        {
            channel1Size = new Vector4D<float>(1.0f / Channel1.Width, 1.0f / Channel1.Height, Channel1.Width, Channel1.Height);
        }
        if (Channel2 != null)
        {
            channel2Size = new Vector4D<float>(1.0f / Channel2.Width, 1.0f / Channel2.Height, Channel2.Width, Channel2.Height);
        }
        if (Channel3 != null)
        {
            channel3Size = new Vector4D<float>(1.0f / Channel3.Width, 1.0f / Channel3.Height, Channel3.Width, Channel3.Height);
        }
        if (Channel4 != null)
        {
            channel4Size = new Vector4D<float>(1.0f / Channel4.Width, 1.0f / Channel4.Height, Channel4.Width, Channel4.Height);
        }

        PointLights pointLights = new();
        for (int i = 0; i < parameters.PointLights.Length; i++)
        {
            pointLights[i] = new PointLight()
            {
                Color = parameters.PointLights[i].Color,
                Position = parameters.PointLights[i].Transform.Position,
                Intensity = parameters.PointLights[i].Intensity,
                Range = parameters.PointLights[i].Range
            };
        }

        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer(InPosition, 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer(InNormal, 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer(InTangent, 3, nameof(TrVertex.Tangent));
            mesh.VertexAttributePointer(InBitangent, 3, nameof(TrVertex.Bitangent));
            mesh.VertexAttributePointer(InColor, 4, nameof(TrVertex.Color));
            mesh.VertexAttributePointer(InTexCoord, 2, nameof(TrVertex.TexCoord));

            _uboTransforms.SetData(new UniTransforms()
            {
                Model = model,
                View = parameters.Camera.View,
                Projection = parameters.Camera.Projection,
                ObjectToWorld = model,
                ObjectToClip = model * parameters.Camera.View * parameters.Camera.Projection,
                WorldToObject = model.Invert()
            });
            _uboVectors.SetData(new UniVectors()
            {
                Resolution = parameters.SceneData.Resolution,
                CameraPosition = parameters.Camera.Transform.Position,
                CameraUp = parameters.Camera.Transform.Up,
                CameraRight = parameters.Camera.Transform.Right,
                Mouse = parameters.SceneData.Mouse,
                Date = parameters.SceneData.Date
            });
            _uboConstants.SetData(new UniConstants()
            {
                Time = parameters.SceneData.Time,
                DeltaTime = parameters.SceneData.DeltaTime,
                FrameRate = parameters.SceneData.FrameRate,
                FrameCount = parameters.SceneData.FrameCount
            });
            _uboAmbientLight.SetData(new UniAmbientLight()
            {
                Color = parameters.AmbientLight.Color
            });
            _uboDirectionalLight.SetData(new UniDirectionalLight()
            {
                Color = parameters.DirectionalLight.Color,
                Position = -parameters.DirectionalLight.Direction
            });
            _uboPointLights.SetData(new UniPointLights()
            {
                Count = parameters.PointLights.Length,
                Lights = pointLights
            });
            _uboUniTexTexParams.SetData(new UniTexTexParams()
            {
                Channel0Size = channel0Size,
                Channel1Size = channel1Size,
                Channel2Size = channel2Size,
                Channel3Size = channel3Size,
                Channel4Size = channel4Size
            });
            _uboTexScaleOffset.SetData(new UniTexScaleOffset()
            {
                Channel0ST = Channel0ST,
                Channel1ST = Channel1ST,
                Channel2ST = Channel2ST,
                Channel3ST = Channel3ST,
                Channel4ST = Channel4ST
            });

            renderPipeline.BindUniformBlock(0, _uboTransforms);
            renderPipeline.BindUniformBlock(1, _uboVectors);
            renderPipeline.BindUniformBlock(2, _uboConstants);
            renderPipeline.BindUniformBlock(3, _uboAmbientLight);
            renderPipeline.BindUniformBlock(4, _uboDirectionalLight);
            renderPipeline.BindUniformBlock(5, _uboPointLights);
            renderPipeline.BindUniformBlock(6, _uboUniTexTexParams);
            renderPipeline.BindUniformBlock(7, _uboTexScaleOffset);

            renderPipeline.BindUniformBlock(0, Channel0);
            renderPipeline.BindUniformBlock(1, Channel1);
            renderPipeline.BindUniformBlock(2, Channel2);
            renderPipeline.BindUniformBlock(3, Channel3);
            renderPipeline.BindUniformBlock(4, Channel4);

            renderPipeline.Unbind();
        }

        DrawCore(mesh, parameters);
    }

    public void AdjustChannel(string label, int index)
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

        TrTexture? channel = (TrTexture?)cache.Channel.GetValue(this);
        Vector4D<float> channelST = (Vector4D<float>)cache.ChannelST.GetValue(this)!;

        ImGui.PushID(label);
        {
            ImGui.Dummy(new Vector2(0.0f, 2.0f));
            {
                float width = ImGui.GetContentRegionAvail().X - ImGui.GetCursorPosX();
                float stWidth = width * 0.6f;

                ImGui.Columns(2, false);
                ImGui.SetColumnWidth(0, stWidth);

                ImGui.BeginGroup();
                {
                    ImGui.PushItemWidth(-1.0f);

                    ImGui.Text(label);

                    ImGui.Indent();
                    {
                        ImGui.Text("Tiling");
                        Vector2 s = new(channelST.X, channelST.Y);
                        ImGui.DragFloat2("##Tiling", ref s, 0.01f);

                        ImGui.Text("Offset");
                        Vector2 t = new(channelST.Z, channelST.W);
                        ImGui.DragFloat2("##Offset", ref t, 0.01f);

                        channelST = new(s.X, s.Y, t.X, t.Y);
                    }
                    ImGui.Unindent();

                    ImGui.PopItemWidth();
                }
                ImGui.EndGroup();

                ImGui.NextColumn();

                Vector2D<float> imageSize = new(ImGui.GetItemRectSize().Y);
                float offsetX = width - stWidth - imageSize.X;

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX + 1.0f);

                TrTextureManager.TextureSelection(label, imageSize, ref channel);

                ImGui.Columns(1);
            }
            ImGui.Dummy(new Vector2(0.0f, 2.0f));
        }
        ImGui.PopID();

        cache.Channel.SetValue(this, channel);
        cache.ChannelST.SetValue(this, channelST);
    }

    protected override void Destroy(bool disposing = false)
    {
        RenderPass.Dispose();

        _uboTransforms.Dispose();
        _uboVectors.Dispose();
        _uboConstants.Dispose();
        _uboAmbientLight.Dispose();
        _uboDirectionalLight.Dispose();
        _uboUniTexTexParams.Dispose();
        _uboTexScaleOffset.Dispose();

        DestroyCore(disposing);
    }

    protected abstract void DrawCore(TrMesh mesh, GlobalParameters globalParameters);

    /// <summary>
    /// 此处应该清理材质中用到的其他缓冲区资源。
    /// 不要在该函数中清理渲染管道和管线。
    /// </summary>
    /// <param name="disposing"></param>
    protected abstract void DestroyCore(bool disposing = false);
}
