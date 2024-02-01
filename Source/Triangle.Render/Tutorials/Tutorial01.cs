using System.ComponentModel;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Models;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter5;
using Triangle.Render.Models;

namespace Triangle.Render.Tutorials;

[DisplayName("简单场景")]
[Description("使用 SimpleMat 材质渲染一个球体并显示法线方向。")]
public class Tutorial01(IInputContext input, TrContext context, string name) : BaseTutorial(input, context, name)
{
    #region Meshes
    private TrMesh sphere = null!;
    #endregion

    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    protected override void Loaded()
    {
        sphere = Context.AssimpParsing("Resources/Models/Sphere.glb".PathFormatter())[0];

        simpleMat = new(Context);

        TransformController.Add("Sphere");
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetParameters("Sphere");

        simpleMat.Draw(sphere, parameters);
    }

    protected override void EditProperties()
    {
        simpleMat.AdjustImGuiProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();
        sphere.Dispose();
    }
}
