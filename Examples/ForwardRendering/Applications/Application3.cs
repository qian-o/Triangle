using Common.Contracts;
using Common.Models;
using Common.Structs;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Numerics;
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

    #region Transforms
    private Vector3D<float> translation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> rotation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> scale = new(10.0f, 10.0f, 10.0f);
    #endregion

    #region Materials
    private DiffuseVertexLevelMat diffuseVertexLevelMat = null!;
    #endregion

    #region Lights
    private TrAmbientLight ambientLight = new(new Vector3D<float>(0.498f, 0.514f, 0.545f));

    private TrDirectionalLight directionalLight = new(new Vector3D<float>(0.0f, 0.0f, 1.0f), new Vector3D<float>(1.0f, 0.9569f, 0.8392f));
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

        Matrix4X4<float> model = Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateScale(scale) * Matrix4X4.CreateTranslation(translation);

        TrParameter parameter = new(main.Camera, model, ambientLight, directionalLight);

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

        Vector3 v1 = translation.ToSystem();
        ImGui.DragFloat3("Translation", ref v1, 0.01f);
        translation = v1.ToGeneric();

        Vector3 v2 = rotation.ToSystem();
        ImGui.DragFloat3("Rotation", ref v2, 0.01f);
        rotation = v2.ToGeneric();

        Vector3 v3 = scale.ToSystem();
        ImGui.DragFloat3("Scale", ref v3, 0.01f);
        scale = v3.ToGeneric();

        Vector3 ambient = ambientLight.Color.ToSystem();
        ImGui.ColorEdit3("Ambient Light", ref ambient);
        ambientLight.Color = ambient.ToGeneric();

        Vector3 directional = directionalLight.Color.ToSystem();
        ImGui.ColorEdit3("Directional Light", ref directional);
        directionalLight.Color = directional.ToGeneric();

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
