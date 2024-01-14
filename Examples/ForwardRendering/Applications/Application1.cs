using System.Numerics;
using Common.Contracts;
using Common.Models;
using ForwardRendering.Materials.Chapter5;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application1 : BaseApplication
{
    #region Viewports
    private TrScene main = null!;
    #endregion

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


    public override void Loaded()
    {
        main = new(Input, Context, "Main");

        cube = Context.CreateCube();

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

        Matrix4X4<float> model = Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateScale(scale) * Matrix4X4.CreateTranslation(translation);

        TrParameter parameter = new(main.Camera, model);

        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(cube, parameter);

        main.End();
    }

    public override void ImGuiRender()
    {
        main.DrawHost();

        if (ImGui.Begin("Properties"))
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

            simpleMat.ImGuiEdit();

            ImGui.End();
        }
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();

        cube.Dispose();

        main.Dispose();
    }
}
