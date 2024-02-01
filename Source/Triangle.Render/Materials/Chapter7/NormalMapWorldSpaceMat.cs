using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Chapter7;

public class NormalMapWorldSpaceMat(TrContext context) : GlobalMat(context, "NormalMapWorldSpace")
{
    public override TrRenderPass CreateRenderPass()
    {
        throw new NotImplementedException();
    }

    protected override void DrawCore(TrMesh mesh, GlobalParameters globalParameters)
    {
        throw new NotImplementedException();
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        throw new NotImplementedException();
    }

    protected override void DestroyCore(bool disposing = false)
    {
        throw new NotImplementedException();
    }
}
