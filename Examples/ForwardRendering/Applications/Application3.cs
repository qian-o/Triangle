using Common.Contracts;
using Common.Models;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application3 : BaseApplication
{
    #region Viewports
    private TrViewport main = null!;
    #endregion

    #region Meshes
    private TrMesh goldStar = null!;
    #endregion

    #region Materials
    private DiffuseVertexLevelMat diffuseVertexLevelMat = null!;
    #endregion

    public override void Loaded()
    {
        main = new(Input, Context, "Main");

        goldStar = TrMeshFactory.AssimpParsing(Context, "Resources/Models/Gold Star.glb")[0];

        diffuseVertexLevelMat = new(Context);
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

        diffuseVertexLevelMat.Draw(goldStar, parameter);

        main.End();
    }

    public override void DrawImGui()
    {
        main.DrawHost();

        ImGui.Begin("Properties");

        diffuseVertexLevelMat.ImGuiEdit();

        ImGui.End();
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        diffuseVertexLevelMat.Dispose();

        goldStar.Dispose();

        main.Dispose();
    }
}
