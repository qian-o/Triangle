using Common;
using Common.Contracts;
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

public class Application2 : BaseApplication
{
    #region Meshes
    private TrMesh grid = null!;
    private TrMesh cube = null!;
    #endregion

    #region Materials
    private GridMat gridMat = null!;
    private SimpleMat simpleMat = null!;
    #endregion

    public override void Initialize([NotNull] IWindow window, [NotNull] TrContext context, [NotNull] Camera camera)
    {
        base.Initialize(window, context, camera);

        grid = TrMeshFactory.CreateGrid(Context);
        cube = TrMeshFactory.CreateCube(Context);

        gridMat = new(Context);
        simpleMat = new(Context);
    }

    public override void Update(double deltaSeconds)
    {
    }

    public override void Render([NotNull] TrFrame frame, double deltaSeconds)
    {
        GL gl = Context.GL;

        frame.Bind();

        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(cube, Camera, Matrix4X4<float>.Identity);
        gridMat.Draw(grid, Camera);

        frame.Unbind();
    }

    public override void DrawImGui()
    {
        gridMat.ImGuiEdit();
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
    }
}
