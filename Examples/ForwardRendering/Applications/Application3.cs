using Common;
using Common.Contracts;
using Common.Models;
using ForwardRendering.Materials;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application3 : BaseApplication
{
    #region Meshes
    private TrMesh goldStar = null!;
    #endregion

    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    public override void Initialize([NotNull] IWindow window, [NotNull] TrContext context, [NotNull] Camera camera)
    {
        base.Initialize(window, context, camera);

        goldStar = TrMeshFactory.AssimpParsing(Context, "Resources/Models/Gold Star.glb")[0];

        simpleMat = new(Context);
    }

    public override void Update(double deltaSeconds)
    {
    }

    public override void Render([NotNull] TrFrame frame, double deltaSeconds)
    {
        TrParameter parameter = new(Camera, Matrix4X4<float>.Identity);

        GL gl = Context.GL;

        frame.Bind();

        gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(goldStar, parameter);

        frame.Unbind();
    }

    public override void DrawImGui()
    {
        simpleMat.ImGuiEdit();
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    public override void FramebufferResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();

        goldStar.Dispose();
    }
}
