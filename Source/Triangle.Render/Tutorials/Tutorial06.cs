using System.ComponentModel;
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
    private TrMesh knot = null!;
    #endregion

    #region Materials
    private RampTextureMat rampTextureMat = null!;
    #endregion

    protected override void Loaded()
    {
        knot = Context.AssimpParsing("Resources/Models/Knot.FBX".PathFormatter())[0];

        rampTextureMat = new(Context);

        TransformController.Add("Knot");

        TransformController.SetTransform("Knot", new Vector3D<float>(0, 2.0f, 0), new Vector3D<float>(90.0f, 180.0f, 0), new Vector3D<float>(0.05f, 0.05f, 0.05f));
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetParameters("Knot");

        rampTextureMat.Draw(knot, parameters);
    }

    protected override void EditProperties()
    {
        rampTextureMat.AdjustProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        rampTextureMat.Dispose();
        knot.Dispose();
    }
}
