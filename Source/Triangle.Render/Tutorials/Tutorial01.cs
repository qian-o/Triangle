using System.ComponentModel;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter5;

namespace Triangle.Render.Tutorials;

[DisplayName("Simple Material")]
[Description("Use simple material to render.")]
public class Tutorial01(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    #region Models
    private TrModel sphere = null!;
    #endregion

    protected override void Loaded()
    {
        simpleMat = new SimpleMat(Context);

        SceneController.Add(sphere = new("Sphere", [Context.GetSphere()], simpleMat));
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        sphere.Render(Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();
    }
}
