using System.ComponentModel;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter5;

namespace Triangle.Render.Tutorials;

[DisplayName("简单场景")]
[Description("使用 SimpleMat 材质渲染一个球体并显示法线方向。")]
public class Tutorial01(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh[] sphereMeshes = null!;
    #endregion

    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    #region Models
    private MeshModel sphere = null!;
    #endregion

    protected override void Loaded()
    {
        sphereMeshes = Context.AssimpParsing("Resources/Models/Sphere.glb".PathFormatter());

        simpleMat = new(Context);

        SceneController.Add(sphere = new("Sphere", sphereMeshes, simpleMat));
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        sphere.Render(GetSceneParameters());
    }

    protected override void EditProperties()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();

        sphereMeshes.Dispose();
    }
}
