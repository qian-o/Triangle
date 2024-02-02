using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;
using Triangle.Render.Models;

namespace Triangle.Render.Tutorials;

[DisplayName("漫反射")]
[Description("使用 Diffuse 相关材质渲染五角星。")]
public class Tutorial02(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh goldStar = null!;
    #endregion

    #region Materials
    private DiffuseVertexLevelMat diffuseVertexLevelMat = null!;
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    private HalfLambertMat halfLambertMat = null!;
    #endregion

    protected override void Loaded()
    {
        goldStar = Context.AssimpParsing("Resources/Models/Gold Star.glb".PathFormatter())[0];

        diffuseVertexLevelMat = new(Context);
        diffusePixelLevelMat = new(Context);
        halfLambertMat = new(Context);

        TransformController.Add("Gold Star");
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetParameters("Gold Star");

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(-2.0f, 0.0f, 0.0f));
        diffuseVertexLevelMat.Draw(goldStar, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(2.0f, 0.0f, 0.0f));
        diffusePixelLevelMat.Draw(goldStar, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(2.0f, 0.0f, 0.0f));
        halfLambertMat.Draw(goldStar, parameters);
    }

    protected override void EditProperties()
    {
        diffuseVertexLevelMat.AdjustImGuiProperties();
        diffusePixelLevelMat.AdjustImGuiProperties();
        halfLambertMat.AdjustImGuiProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        halfLambertMat.Dispose();
        diffusePixelLevelMat.Dispose();
        diffuseVertexLevelMat.Dispose();
        goldStar.Dispose();
    }
}
