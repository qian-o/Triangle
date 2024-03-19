using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("漫反射")]
[Description("使用 Diffuse 相关材质渲染五角星。")]
public class Tutorial02(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Models
    private TrModel star1 = null!;
    private TrModel star2 = null!;
    private TrModel star3 = null!;
    #endregion

    protected override void Loaded()
    {
        star1 = new("Star 1", [Context.GetStar()], new DiffuseVertexLevelMat(Context));
        star1.Transform.Translate(new Vector3D<float>(-2.0f, 0.0f, 0.0f));

        star2 = new("Star 2", [Context.GetStar()], new DiffusePixelLevelMat(Context));
        star2.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, 0.0f));

        star3 = new("Star 3", [Context.GetStar()], new HalfLambertMat(Context));
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
        star1.Render(GetSceneParameters());
        star2.Render(GetSceneParameters());
        star3.Render(GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
        star1.Dispose();
        star2.Dispose();
        star3.Dispose();
    }
}
