using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Diffuse")]
[Description("Use diffuse to render.")]
public class Tutorial02(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Materials
    private DiffuseVertexLevelMat diffuseVertexLevelMat = null!;
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    private HalfLambertMat halfLambertMat = null!;
    #endregion

    #region Models
    private TrModel star1 = null!;
    private TrModel star2 = null!;
    private TrModel star3 = null!;
    #endregion

    protected override void Loaded()
    {
        diffuseVertexLevelMat = new DiffuseVertexLevelMat(Context);
        diffusePixelLevelMat = new DiffusePixelLevelMat(Context);
        halfLambertMat = new HalfLambertMat(Context);

        star1 = new("Star 1", [Context.GetStar()], diffuseVertexLevelMat);
        star1.Transform.Translate(new Vector3D<float>(-2.0f, 0.0f, 0.0f));

        star2 = new("Star 2", [Context.GetStar()], diffusePixelLevelMat);
        star2.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, 0.0f));

        star3 = new("Star 3", [Context.GetStar()], halfLambertMat);
        star3.Transform.Translate(new Vector3D<float>(2.0f, 0.0f, 0.0f));

        SceneController.Add(star1);
        SceneController.Add(star2);
        SceneController.Add(star3);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        star1.Render(Parameters);
        star2.Render(Parameters);
        star3.Render(Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        diffuseVertexLevelMat.Dispose();
        diffusePixelLevelMat.Dispose();
        halfLambertMat.Dispose();
    }
}
