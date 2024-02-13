using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;

namespace Triangle.Render.Tutorials;

[DisplayName("2D 纹理")]
[Description("使用 Texture 相关材质渲染胶囊体。")]
public class Tutorial04(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh[] capsuleMeshes = null!;
    #endregion

    #region Materials
    private SingleTextureMat singleTextureMat = null!;
    private NormalMapWorldSpaceMat normalMapWorldSpaceMat = null!;
    private NormalMapTangentSpaceMat normalMapTangentSpaceMat = null!;
    private MaskTextureMat maskTextureMat = null!;
    #endregion

    #region Models
    private MeshModel capsule1 = null!;
    private MeshModel capsule2 = null!;
    private MeshModel capsule3 = null!;
    private MeshModel capsule4 = null!;
    #endregion

    protected override void Loaded()
    {
        capsuleMeshes = Context.AssimpParsing("Resources/Models/Capsule.glb".PathFormatter());

        singleTextureMat = new(Context);
        normalMapWorldSpaceMat = new(Context);
        normalMapTangentSpaceMat = new(Context);
        maskTextureMat = new(Context);

        capsule1 = new(TransformController, "Capsule 1", capsuleMeshes, singleTextureMat);
        capsule1.SetTranslation(new Vector3D<float>(-4.5f, 0.0f, 0.0f));

        capsule2 = new(TransformController, "Capsule 2", capsuleMeshes, normalMapWorldSpaceMat);
        capsule2.SetTranslation(new Vector3D<float>(-1.5f, 0.0f, 0.0f));

        capsule3 = new(TransformController, "Capsule 3", capsuleMeshes, normalMapTangentSpaceMat);
        capsule3.SetTranslation(new Vector3D<float>(1.5f, 0.0f, 0.0f));

        capsule4 = new(TransformController, "Capsule 4", capsuleMeshes, maskTextureMat);
        capsule4.SetTranslation(new Vector3D<float>(4.5f, 0.0f, 0.0f));

        PickupController.Add(capsule1);
        PickupController.Add(capsule2);
        PickupController.Add(capsule3);
        PickupController.Add(capsule4);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        capsule1.Render(GetSceneParameters());
        capsule2.Render(GetSceneParameters());
        capsule3.Render(GetSceneParameters());
        capsule4.Render(GetSceneParameters());
    }

    protected override void EditProperties()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        singleTextureMat.Dispose();
        normalMapWorldSpaceMat.Dispose();
        normalMapTangentSpaceMat.Dispose();
        maskTextureMat.Dispose();

        capsuleMeshes.Dispose();
    }
}
