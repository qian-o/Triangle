using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;
using Triangle.Render.Materials.Shadertoy;
using Triangle.Render.Models;

namespace Triangle.Render.Tutorials;

[DisplayName("Shadertoy")]
[Description("从 Shadertoy 上移植的着色器。")]
public class Tutorial05(IInputContext input, TrContext context, string name) : BaseTutorial(input, context, name)
{
    #region Meshes
    private TrMesh canvas = null!;
    #endregion

    #region Materials
    private FullSpectrumCyberMat fullSpectrumCyberMat = null!;
    #endregion

    protected override void Loaded()
    {
        canvas = Context.CreateCanvas();

        fullSpectrumCyberMat = new(Context);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = new(Scene.Camera, Matrix4X4<float>.Identity, Scene.SceneData);

        fullSpectrumCyberMat.Draw(canvas, parameters);
    }

    protected override void EditProperties()
    {

    }

    protected override void Destroy(bool disposing = false)
    {

    }
}
