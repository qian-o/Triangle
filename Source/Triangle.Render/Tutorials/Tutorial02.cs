using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("漫反射")]
[Description("使用 Diffuse 相关材质渲染五角星。")]
public class Tutorial02(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh[] goldStarMeshes = null!;
    #endregion

    #region Materials
    private DiffuseVertexLevelMat diffuseVertexLevelMat = null!;
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    private HalfLambertMat halfLambertMat = null!;
    #endregion

    #region Models
    private MeshModel goldStar1 = null!;
    private MeshModel goldStar2 = null!;
    private MeshModel goldStar3 = null!;
    #endregion

    protected override void Loaded()
    {
        goldStarMeshes = Context.AssimpParsing("Resources/Models/Gold Star.glb".PathFormatter());

        diffuseVertexLevelMat = new(Context);
        diffusePixelLevelMat = new(Context);
        halfLambertMat = new(Context);

        goldStar1 = new("Gold Star 1", goldStarMeshes, diffuseVertexLevelMat);
        goldStar1.Transform.Translate(new Vector3D<float>(-2.0f, 0.0f, 0.0f));

        goldStar2 = new("Gold Star 2", goldStarMeshes, diffusePixelLevelMat);
        goldStar2.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, 0.0f));

        goldStar3 = new("Gold Star 3", goldStarMeshes, halfLambertMat);
        goldStar3.Transform.Translate(new Vector3D<float>(2.0f, 0.0f, 0.0f));

        SceneController.Add(goldStar1);
        SceneController.Add(goldStar2);
        SceneController.Add(goldStar3);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        goldStar1.Render(GetSceneParameters());
        goldStar2.Render(GetSceneParameters());
        goldStar3.Render(GetSceneParameters());
    }

    protected override void EditProperties()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        halfLambertMat.Dispose();
        diffusePixelLevelMat.Dispose();
        diffuseVertexLevelMat.Dispose();

        goldStarMeshes.Dispose();
    }
}
