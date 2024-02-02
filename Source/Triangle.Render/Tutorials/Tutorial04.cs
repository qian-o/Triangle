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

[DisplayName("2D 纹理")]
[Description("使用 Texture 相关材质渲染胶囊体。")]
public class Tutorial04(IInputContext input, TrContext context, string name) : BaseTutorial(input, context, name)
{
    #region Meshes
    private TrMesh capsule = null!;
    #endregion

    #region Materials
    private SingleTextureMat singleTextureMat = null!;
    private NormalMapWorldSpaceMat normalMapWorldSpaceMat = null!;
    private NormalMapTangentSpaceMat normalMapTangentSpaceMat = null!;
    #endregion

    protected override void Loaded()
    {
        capsule = Context.AssimpParsing("Resources/Models/Capsule.glb".PathFormatter())[0];

        singleTextureMat = new(Context);
        normalMapWorldSpaceMat = new(Context);
        normalMapTangentSpaceMat = new(Context);

        TransformController.Add("Capsule");
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetParameters("Capsule");

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(-3.0f, 0.0f, 0.0f));
        singleTextureMat.Draw(capsule, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(3.0f, 0.0f, 0.0f));
        normalMapWorldSpaceMat.Draw(capsule, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(3.0f, 0.0f, 0.0f));
        normalMapTangentSpaceMat.Draw(capsule, parameters);
    }

    protected override void EditProperties()
    {
        singleTextureMat.AdjustImGuiProperties();
        normalMapWorldSpaceMat.AdjustImGuiProperties();
        normalMapTangentSpaceMat.AdjustImGuiProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        singleTextureMat.Dispose();
        normalMapWorldSpaceMat.Dispose();
        normalMapTangentSpaceMat.Dispose();
        capsule.Dispose();
    }
}
