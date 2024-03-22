using System.ComponentModel;
using Hexa.NET.ImGui;
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
    private TrFrame debugFrame = null!;
    #endregion

    #region Materials
    private ShadowMappingDepthMat shadowMappingDepthMat = null!;
    private DepthDebugMat depthDebugMat = null!;
    private DiffusePixelLevelMat diffusePixelLevelMat = null!;
    private ShadowMappingMat shadowMappingMat = null!;
    #endregion

    #region Meshes
    private TrMesh canvas = null!;
    private TrMesh cube = null!;
    #endregion

    #region Models
    private TrModel floor = null!;
    private TrModel wall = null!;
    private TrModel[] cubes = null!;
    #endregion

    protected override void Loaded()
    {
        depthFrame = new(Context);
        debugFrame = new(Context);

        shadowMappingDepthMat = new(Context);
        depthDebugMat = new(Context);
        diffusePixelLevelMat = new(Context)
        {
            Diffuse = new Vector4D<float>(0.7960f, 0.7960f, 0.7960f, 1.0f)
        };
        shadowMappingMat = new(Context);

        canvas = Context.GetCanvas();
        cube = Context.GetCube();

        floor = new("Floor", [cube], diffusePixelLevelMat);
        floor.Transform.Translate(new Vector3D<float>(0.0f, -0.5f, 0.0f));
        floor.Transform.Scaled(new Vector3D<float>(20.0f, 1.0f, 20.0f));

        wall = new("Wall", [cube], diffusePixelLevelMat);
        wall.Transform.Translate(new Vector3D<float>(10.5f, 4.0f, 0.0f));
        wall.Transform.Rotate(new Vector3D<float>(0.0f, 90.0f, 0.0f));
        wall.Transform.Scaled(new Vector3D<float>(20.0f, 10.0f, 1.0f));

        SceneController.Add(floor);
        SceneController.Add(wall);

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
        Matrix4X4<float> lightProjection = Matrix4X4.CreateOrthographic(40.0f, 40.0f, 0.0f, 100.0f);
        Matrix4X4<float> lightView = Matrix4X4.CreateLookAt(new Vector3D<float>(0.0f, 0.0f, 0.0f), DirectionalLight.Direction, new Vector3D<float>(0.0f, 1.0f, 0.0f));
        Matrix4X4<float> lightSpace = lightView * lightProjection;

        depthFrame.Update(4096, 4096);
        depthFrame.Bind();
        {
            Context.Clear();

            shadowMappingDepthMat.LightSpace = lightSpace;
            shadowMappingDepthMat.Draw([floor, wall, .. cubes], [Parameters]);
        }
        depthFrame.Unbind();

        debugFrame.Update(4096, 4096);
        debugFrame.Bind();
        {
            Context.Clear();

            depthDebugMat.NearPlane = 0.0f;
            depthDebugMat.FarPlane = 100.0f;
            depthDebugMat.Channel0 = depthFrame.Texture;
            depthDebugMat.Draw([canvas], [Parameters]);
        }
        debugFrame.Unbind();

        diffusePixelLevelMat.Draw([floor, wall, .. cubes], [Parameters]);

        shadowMappingMat.LightSpace = lightSpace;
        shadowMappingMat.Channel0 = depthFrame.Texture;
        shadowMappingMat.Draw([floor, wall, .. cubes], [Parameters]);
    }

    public override void ImGuiRender()
    {
        base.ImGuiRender();

        ImGui.Begin("Debug");
        {
            ImGuiHelper.Frame(debugFrame);
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        depthFrame.Dispose();

        shadowMappingDepthMat.Dispose();
        diffusePixelLevelMat.Dispose();
    }
}
