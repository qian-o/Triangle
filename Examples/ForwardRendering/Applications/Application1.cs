using Common;
using Common.Contracts;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application1 : BaseApplication
{
    #region Meshes
    private TrMesh cube = null!;
    #endregion

    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    #region Transform
    private Vector3D<float> translation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> rotation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> scale = new(1.0f, 1.0f, 1.0f);
    #endregion

    public IWindow Window { get; private set; } = null!;

    public TrContext Context { get; private set; } = null!;

    public Camera Camera { get; private set; } = null!;

    public override void Initialize(IWindow window, TrContext context, Camera camera)
    {
        Window = window;
        Context = context;
        Camera = camera;

        simpleMat = new(Context);

        cube = TrMeshFactory.CreateCube(Context, 0.5f);
    }

    public override void Update(double deltaSeconds)
    {
    }

    public override void Render([NotNull] TrFrame frame, double deltaSeconds)
    {
        GL gl = Context.GL;

        frame.Bind();

        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(cube, Camera, Matrix4X4.CreateScale(scale) * Matrix4X4.CreateTranslation(translation));

        frame.Unbind();
    }

    public override void DrawImGui()
    {
        Vector3 v1 = translation.ToSystem();
        ImGui.DragFloat3("Translation", ref v1, 0.01f);
        translation = v1.ToGeneric();

        Vector3 v2 = rotation.ToSystem();
        ImGui.DragFloat3("Rotation", ref v2, 0.01f);
        rotation = v2.ToGeneric();

        Vector3 v3 = scale.ToSystem();
        ImGui.DragFloat3("Scale", ref v3, 0.01f);
        scale = v3.ToGeneric();

        Vector4 v4 = simpleMat.Color.ToSystem();
        ImGui.ColorEdit4("SimpleMat.Color", ref v4);
        simpleMat.Color = v4.ToGeneric();
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    public override void FramebufferResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        cube.Dispose();
        simpleMat.Dispose();
    }
}
