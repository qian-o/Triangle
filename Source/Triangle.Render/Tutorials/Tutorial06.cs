using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;

namespace Triangle.Render.Tutorials;

[DisplayName("Ramp Texture")]
[Description("Ramp texture achieves stylized shading.")]
public class Tutorial06(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Materials
    private RampTextureMat rampTextureMat = null!;
    #endregion

    #region Models
    private TrModel knot = null!;
    #endregion

    protected override void Loaded()
    {
        rampTextureMat = new(Context);

        knot = new("Knot", Context.AssimpParsing("Resources/Models/Knot.FBX".Path()), rampTextureMat);
        knot.Transform.Translate(new Vector3D<float>(0.0f, 2.0f, 0.0f));
        knot.Transform.Rotate(new Vector3D<float>(90.0f, 180.0f, 0.0f));
        knot.Transform.Scaled(new Vector3D<float>(0.05f, 0.05f, 0.05f));

        SceneController.Add(knot);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        knot.Render(Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        rampTextureMat.Dispose();
    }
}
