using System.ComponentModel;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Shadertoy;

namespace Triangle.Render.Tutorials;

[DisplayName("Shadertoy")]
[Description("从 Shadertoy 上移植的着色器, 鼠标中键按下切换着色器。")]
public class Tutorial05(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh canvas = null!;
    #endregion

    #region Materials
    private int materialIndex;
    private GlobalMat[] mats = null!;
    #endregion

    protected override void Loaded()
    {
        Scene.UseTools = false;

        canvas = Context.CreateCanvas();

        mats =
        [
            new BoxFrameDistance3DMat(Context),
            new RaymarchingPrimitivesMat(Context),
            new FullSpectrumCyberMat(Context)
        ];
    }

    protected override void UpdateScene(double deltaSeconds)
    {
        if (Scene.IsMiddleClicked)
        {
            materialIndex = (materialIndex + 1) % mats.Length;
        }
    }

    protected override void RenderScene(double deltaSeconds)
    {
        mats[materialIndex].Draw([canvas], GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
        foreach (GlobalMat mat in mats)
        {
            mat.Dispose();
        }

        canvas.Dispose();
    }
}
