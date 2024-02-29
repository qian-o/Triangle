using Silk.NET.Maths;
using Triangle.Core.Graphics;

namespace Triangle.Core.Materials;

internal sealed class AmbientLightMat(TrContext context) : TrMaterial(context, "AmbientLight")
{
    public Vector3D<float> Color { get; set; }

    public override TrRenderPass CreateRenderPass()
    {
        return null;
    }

    public override void Draw(TrMesh mesh, params object[] args)
    {
    }

    protected override void ControllerCore()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
