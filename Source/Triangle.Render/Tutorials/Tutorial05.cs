using System.ComponentModel;
using ImGuiNET;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Shadertoy;

namespace Triangle.Render.Tutorials;

[DisplayName("Shadertoy")]
[Description("从 Shadertoy 上移植的着色器。")]
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
        mats[materialIndex].Draw(canvas, GetSceneParameters());
    }

    protected override void EditProperties()
    {
        if (ImGui.TreeNode("Material"))
        {
            ImGui.Combo("##Material", ref materialIndex, mats.Select(item => item.Name).ToArray(), mats.Length);

            ImGui.TreePop();
        }
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
