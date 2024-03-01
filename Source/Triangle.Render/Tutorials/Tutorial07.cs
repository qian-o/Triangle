using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials;

namespace Triangle.Render.Tutorials;

[DisplayName("PBR 渲染")]
[Description("Physically Based Rendering (PBR) 渲染光照模型。")]
public class Tutorial07(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Models
    private TrModel[] spheres = null!;
    #endregion
    protected override void Loaded()
    {
        const int rows = 7;
        const int cols = 7;
        const float spacing = 2.5f;

        spheres = new TrModel[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * rows + j;

                spheres[index] = new($"Sphere [{index}]", [Context.CreateSphere()], new PhysicallyBasedRenderingMat(Context));
                spheres[index].Transform.Translate(new Vector3D<float>(i * spacing - rows * spacing / 2.0f, j * spacing - cols * spacing / 2.0f, 0.0f));
                SceneController.Add(spheres[index]);
            }
        }

        AddPointLight("Point Light [0]", out TrPointLight pointLight0);
        pointLight0.Transform.Translate(new Vector3D<float>(-2.0f, 2.0f, 4.0f));

        AddPointLight("Point Light [1]", out TrPointLight pointLight1);
        pointLight1.Transform.Translate(new Vector3D<float>(2.0f, 2.0f, 4.0f));

        AddPointLight("Point Light [2]", out TrPointLight pointLight2);
        pointLight2.Transform.Translate(new Vector3D<float>(-2.0f, -2.0f, 4.0f));

        AddPointLight("Point Light [3]", out TrPointLight pointLight3);
        pointLight3.Transform.Translate(new Vector3D<float>(2.0f, -2.0f, 4.0f));
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        foreach (var sphere in spheres)
        {
            sphere.Render(GetSceneParameters());
        }
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
