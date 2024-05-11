using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Scene Model")]
[Description("This tutorial demonstrates how to load a scene model and render it.")]
public class Tutorial10(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Materials
    private DiffusePixelLevelMat[] diffusePixelLevelMats = null!;
    #endregion

    #region Models
    private TrModel battle = null!;
    #endregion

    protected override void Loaded()
    {
        (TrMesh[] Meshes, TrMaterialProperty[] MaterialProperties) = Context.AssimpParsing(Path.Combine("Resources", "Models", "Battle of the Trash god.fbx"));

        diffusePixelLevelMats = new DiffusePixelLevelMat[MaterialProperties.Length];
        for (int i = 0; i < MaterialProperties.Length; i++)
        {
            diffusePixelLevelMats[i] = new DiffusePixelLevelMat(Context)
            {
                Diffuse = MaterialProperties[i].DiffuseColor
            };
        }

        battle = new("Battle of the Trash god", Meshes, diffusePixelLevelMats);
        battle.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, -75.0f));
        battle.Transform.Rotate(new Vector3D<float>(0.0f, 89.9f, 0.0f));
        battle.Transform.Scaled(new Vector3D<float>(0.01f, 0.01f, 0.01f));

        SceneController.Add(battle);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        battle.Render([Parameters]);
    }

    protected override void Destroy(bool disposing = false)
    {
        foreach (DiffusePixelLevelMat diffusePixelLevelMat in diffusePixelLevelMats)
        {
            diffusePixelLevelMat.Dispose();
        }

        battle.Dispose();
    }
}
