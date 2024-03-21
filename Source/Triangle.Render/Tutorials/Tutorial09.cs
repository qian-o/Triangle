using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Shadow-Mapping")]
[Description("Show how to use shadow mapping.")]
public class Tutorial09(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Framebuffers
    private TrDepthFrame depthFrame = null!;
    #endregion

    #region Materials
    private ShadowMappingDepthMat shadowMappingDepthMat = null!;
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    #endregion

    #region Meshes
    private TrMesh cube = null!;
    #endregion

    #region Models
    private TrModel floor = null!;
    private TrModel[] cubes = null!;
    #endregion

    protected override void Loaded()
    {
        depthFrame = new(Context);

        shadowMappingDepthMat = new(Context);

        diffusePixelLevelMat = new(Context)
        {
            Diffuse = new Vector4D<float>(0.7960f, 0.7960f, 0.7960f, 1.0f)
        };

        cube = Context.GetCube();

        floor = new("Floor", [cube], diffusePixelLevelMat);
        floor.Transform.Translate(new Vector3D<float>(0.0f, -0.5f, 0.0f));
        floor.Transform.Scaled(new Vector3D<float>(20.0f, 1.0f, 20.0f));

        SceneController.Add(floor);

        cubes = new TrModel[10];
        for (var i = 0; i < 10; i++)
        {
            cubes[i] = new($"Cube {i}", [cube], diffusePixelLevelMat);
            cubes[i].Transform.Translate(new Vector3D<float>(-3.0f + i, 0.5f, -3.0f + i));
            SceneController.Add(cubes[i]);
        }
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        depthFrame.Update(2048, 2048);
        depthFrame.Bind();
        {
            Context.Clear();
        }
        depthFrame.Unbind();

        diffusePixelLevelMat.Draw([floor, .. cubes], [Parameters]);
    }

    protected override void Destroy(bool disposing = false)
    {
        depthFrame.Dispose();

        shadowMappingDepthMat.Dispose();
    }
}
