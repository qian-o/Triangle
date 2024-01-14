using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Graphics;

namespace Triangle.Render.Graphics;

public abstract class TrMaterial<TParameter> : TrGraphics<TrContext>
{
    private TrRenderPass? renderPass;

    protected TrMaterial(TrContext context) : base(context)
    {
    }

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw([NotNull] TrMesh mesh, [NotNull] TParameter parameter);

    public abstract void AdjustImGuiProperties();
}
