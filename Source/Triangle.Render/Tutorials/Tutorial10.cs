using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Final Tutorial")]
[Description("Demonstrate how to load a 3D model and render it.")]
public class Tutorial10(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Materials
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    #endregion

    #region Models
    private TrModel scene = null!;
    #endregion

    protected override void Loaded()
    {
        //321
        diffusePixelLevelMat = new(Context)
        {
            Diffuse = new Vector4D<float>(0.7960f, 0.7960f, 0.7960f, 1.0f)
        };

        scene = new("Scene", Context.AssimpParsing(Path.Combine("Resources", "Models", "Battle of the Trash god.fbx")), diffusePixelLevelMat);
        scene.Transform.Translate(new Vector3D<float>(0.0f, 0.0f, -75.0f));
        scene.Transform.Rotate(new Vector3D<float>(0.0f, 89.9f, 0.0f));
        scene.Transform.Scaled(new Vector3D<float>(0.01f, 0.01f, 0.01f));

        SceneController.Add(scene);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        diffusePixelLevelMat.Draw([scene], [Parameters]);
    }

    protected override void Destroy(bool disposing = false)
    {
        diffusePixelLevelMat.Dispose();
    }

}
