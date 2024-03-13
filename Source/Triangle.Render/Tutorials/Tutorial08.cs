using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials;

namespace Triangle.Render.Tutorials;

[DisplayName("物理模拟")]
[Description("演示如何使用 BepuPhysics 进行物理模拟。")]
public class Tutorial08(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh cubeMesh = null!;
    #endregion

    #region Materials
    private SolidColorInstancedMat solidColorInstancedMat = null!;
    #endregion

    #region Models
    private TrModel[] cubes = null!;
    #endregion

    protected override void Loaded()
    {
        Scene.Camera.Transform.Position = new Vector3D<float>(50.0f, 15.0f, 100.0f);
        Scene.Camera.Transform.EulerAngles = new Vector3D<float>(0.0f, 40.0f, 0.0f);

        cubeMesh = Context.CreateCube();

        solidColorInstancedMat = new(Context);

        const int columns = 40;
        const int rows = 20;
        List<TrModel> cubesList = [];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int columnCount = rows - j;
                for (int k = 0; k < columnCount; k++)
                {
                    TrModel cube = new($"Cube {cubesList.Count}", [cubeMesh], solidColorInstancedMat);
                    cube.Transform.Position = new Vector3D<float>((-columnCount * 0.5f) + k, j + 0.5f, (i - (columns * 0.5f)) * 4.0f);

                    cubesList.Add(cube);

                    SceneController.Add(cube);
                }
            }
        }
        cubes = [.. cubesList];
    }

    protected override void UpdateScene(double deltaSeconds)
    {
        solidColorInstancedMat.Color = cubes.Select(item => item.ColorId.ToSingle()).ToArray();
    }

    protected override void RenderScene(double deltaSeconds)
    {
        solidColorInstancedMat.Draw(cubes, GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
