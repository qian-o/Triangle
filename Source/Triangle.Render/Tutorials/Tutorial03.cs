using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("高光反射")]
[Description("使用 Specular 相关材质渲染胶囊体。")]
public class Tutorial03(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh[] capsuleMeshes = null!;
    #endregion

    #region Materials
    private SpecularVertexLevelMat specularVertexLevelMat = null!;
    private SpecularPixelLevelMat specularPixelLevelMat = null!;
    private BlinnPhongMat blinnPhongMat = null!;
    #endregion

    #region Models
    private MeshModel capsule1 = null!;
    private MeshModel capsule2 = null!;
    private MeshModel capsule3 = null!;
    #endregion

    protected override void Loaded()
    {
        capsuleMeshes = Context.AssimpParsing("Resources/Models/Capsule.glb".PathFormatter());

        specularVertexLevelMat = new(Context);
        specularPixelLevelMat = new(Context);
        blinnPhongMat = new(Context);

        capsule1 = new("Capsule 1", capsuleMeshes, specularVertexLevelMat);
        capsule1.Transform.Translate(new Vector3D<float>(-3.0f, 0.0f, 0.0f));

        capsule2 = new("Capsule 2", capsuleMeshes, specularPixelLevelMat);
        capsule2.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, 0.0f));

        capsule3 = new("Capsule 3", capsuleMeshes, blinnPhongMat);
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
        capsule1.Render(GetSceneParameters());
        capsule2.Render(GetSceneParameters());
        capsule3.Render(GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
        blinnPhongMat.Dispose();
        specularPixelLevelMat.Dispose();
        specularVertexLevelMat.Dispose();

        capsuleMeshes.Dispose();
    }
}
