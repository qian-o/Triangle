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

[DisplayName("高光反射")]
[Description("使用 Specular 相关材质渲染胶囊体。")]
public class Tutorial03(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh capsule = null!;
    #endregion

    #region Materials
    private SpecularVertexLevelMat specularVertexLevelMat = null!;
    private SpecularPixelLevelMat specularPixelLevelMat = null!;
    private BlinnPhongMat blinnPhongMat = null!;
    #endregion

    protected override void Loaded()
    {
        capsule = Context.AssimpParsing("Resources/Models/Capsule.glb".PathFormatter())[0];

        specularVertexLevelMat = new(Context);
        specularPixelLevelMat = new(Context);
        blinnPhongMat = new(Context);

        TransformController.Add("Capsule");
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = GetParameters("Capsule");

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(-3.0f, 0.0f, 0.0f));
        specularVertexLevelMat.Draw(capsule, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(3.0f, 0.0f, 0.0f));
        specularPixelLevelMat.Draw(capsule, parameters);

        parameters.Model *= Matrix4X4.CreateTranslation(new Vector3D<float>(3.0f, 0.0f, 0.0f));
        blinnPhongMat.Draw(capsule, parameters);
    }

    protected override void EditProperties()
    {
        specularVertexLevelMat.AdjustProperties();
        specularPixelLevelMat.AdjustProperties();
        blinnPhongMat.AdjustProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        blinnPhongMat.Dispose();
        specularPixelLevelMat.Dispose();
        specularVertexLevelMat.Dispose();
        capsule.Dispose();
    }
}
