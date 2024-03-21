using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Specular")]
[Description("Use specular to render.")]
public class Tutorial03(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Models
    private TrModel capsule1 = null!;
    private TrModel capsule2 = null!;
    private TrModel capsule3 = null!;
    #endregion

    protected override void Loaded()
    {
        capsule1 = new("Capsule 1", [Context.GetCapsule()], new SpecularVertexLevelMat(Context));
        capsule1.Transform.Translate(new Vector3D<float>(-3.0f, 0.0f, 0.0f));

        capsule2 = new("Capsule 2", [Context.GetCapsule()], new SpecularPixelLevelMat(Context));
        capsule2.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, 0.0f));

        capsule3 = new("Capsule 3", [Context.GetCapsule()], new BlinnPhongMat(Context));
        capsule3.Transform.Translate(new Vector3D<float>(3.0f, 0.0f, 0.0f));

        SceneController.Add(capsule1);
        SceneController.Add(capsule2);
        SceneController.Add(capsule3);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        capsule1.Render(Parameters);
        capsule2.Render(Parameters);
        capsule3.Render(Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        capsule1.Dispose();
        capsule2.Dispose();
        capsule3.Dispose();
    }
}
