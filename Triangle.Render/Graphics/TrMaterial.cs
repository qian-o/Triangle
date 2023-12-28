using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Graphics;

namespace Triangle.Render.Graphics;

public abstract class TrMaterial : TrGraphics<TrContext>
{
    protected TrMaterial(TrContext context, TrRenderPass renderPass) : base(context)
    {
        RenderPass = renderPass;
    }

    public TrRenderPass RenderPass { get; }

    public abstract void Draw(TrMesh mesh);
}
