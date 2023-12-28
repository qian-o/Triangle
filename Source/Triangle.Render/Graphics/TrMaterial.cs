using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Graphics;

namespace Triangle.Render.Graphics;

public abstract class TrMaterial : TrGraphics<TrContext>
{
    protected TrMaterial(TrContext context) : base(context)
    {
    }

    public TrRenderPass? RenderPass { get; protected set; }

    public abstract void Draw(TrMesh mesh, params object[] args);
}
