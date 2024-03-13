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
    private SolidColorMat solidColorMat = null!;
    #endregion

    #region Models
    private TrModel[] cubes = null!;
    #endregion

    protected override void Loaded()
    {
        cubeMesh = Context.CreateCube();

        solidColorMat = new(Context);

        const int columns = 10;
        const int rows = 20;
        List<TrModel> cubesList = [];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int columnCount = rows - j;
                for (int k = 0; k < columnCount; k++)
                {
                    TrModel cube = new($"Cube {cubesList.Count}", [cubeMesh], solidColorMat);
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
        solidColorMat.Color = new Vector4D<float>(1, 0, 0, 1);
    }

    protected override void RenderScene(double deltaSeconds)
    {
        foreach (TrModel cube in cubes)
        {
            cube.Render(GetSceneParameters());
        }
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
