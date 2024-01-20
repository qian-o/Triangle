using System.ComponentModel;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Contracts.Tutorials;
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
    private int materialIndex;
    private GlobalMat[] mats = null!;
    #endregion

    protected override void Loaded()
    {
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
    }

    protected override void RenderScene(double deltaSeconds)
    {
        GlobalParameters parameters = new(Scene.Camera, Matrix4X4<float>.Identity, Scene.SceneData);

        mats[materialIndex].Draw(canvas, parameters);
    }

    protected override void EditProperties()
    {
        ImGui.Combo("Material", ref materialIndex, mats.Select(item => item.Name).ToArray(), mats.Length);
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
