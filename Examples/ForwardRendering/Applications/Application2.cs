using Common.Contracts;
using Common.Models;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application2 : BaseApplication
{
    #region Viewports
    private TrViewport main = null!;
    #endregion

    #region Meshes
    private TrMesh grid = null!;
    private TrMesh cube = null!;
    #endregion

    #region Materials
    private GridMat gridMat = null!;
    private SimpleMat simpleMat = null!;
    #endregion

    public override void Loaded()
    {
        main = new(Input, Context, "Main");

        grid = TrMeshFactory.CreateGrid(Context);
        cube = TrMeshFactory.CreateCube(Context);

        gridMat = new(Context);
        simpleMat = new(Context);
    }

    public override void Update(double deltaSeconds)
    {
        main.Update(deltaSeconds);
    }

    public override void Render(double deltaSeconds)
    {
        GL gl = Context.GL;

        main.Begin();

        TrParameter parameter = new(main.Camera, Matrix4X4<float>.Identity);

        gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(cube, parameter);
        gridMat.Draw(grid, parameter);

        main.End();
    }

    public override void DrawImGui()
    {
        main.DrawHost();

        ImGui.Begin("Properties");

        gridMat.ImGuiEdit();
        simpleMat.ImGuiEdit();

        ImGui.End();
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        gridMat.Dispose();
        simpleMat.Dispose();

        grid.Dispose();
        cube.Dispose();

        main.Dispose();
    }
}
