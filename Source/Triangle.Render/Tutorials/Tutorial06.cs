using System.ComponentModel;
using ImGuizmoNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;
using Triangle.Render.Models;

namespace Triangle.Render.Tutorials;

[DisplayName("Ramp 贴图")]
[Description("Ramp 贴图实现风格化渲染。")]
public class Tutorial06(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh[] knotMeshes = null!;
    #endregion

    #region Materials
    private RampTextureMat rampTextureMat = null!;
    #endregion

    #region Models
    private MeshModel knot = null!;
    #endregion

    protected override void Loaded()
    {
        knotMeshes = Context.AssimpParsing("Resources/Models/Knot.FBX".PathFormatter());

        rampTextureMat = new(Context);

        knot = new(TransformController, "Knot", knotMeshes, rampTextureMat);
        knot.SetTranslation(new Vector3D<float>(0, 2.0f, 0));
        knot.SetRotationByDegree(new Vector3D<float>(90.0f, 180.0f, 0));
        knot.SetScale(new Vector3D<float>(0.05f, 0.05f, 0.05f));

        PickupController.Add(knot);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetSceneParameters();

        knot.Render(parameters);
    }

    protected override void EditProperties()
    {
        GlobalParameters parameters = GetSceneParameters();

        float[] view = parameters.Camera.View.ToArray();
        float[] projection = parameters.Camera.Projection.ToArray();
        float[] obj = Matrix4X4<float>.Identity.ToArray();

        ImGuizmo.SetRect(0, 0, 1280, 720);
        ImGuizmo.DrawCubes(ref view[0], ref projection[0], ref obj[0], 1);
        ImGuizmo.ViewManipulate(ref view[0], 8.0f, new System.Numerics.Vector2(128.0f, 128.0f), new System.Numerics.Vector2(128, 128), 0x10101010);
    }

    protected override void Destroy(bool disposing = false)
    {
        rampTextureMat.Dispose();

        knotMeshes.Dispose();
    }
}
