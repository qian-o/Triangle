using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Graphics;

namespace Triangle.Render.Graphics;

public abstract class TrMaterial : TrGraphics<TrContext>
{
    private TrRenderPass? renderPass;

    protected TrMaterial(TrContext context) : base(context)
    {
    }

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw([NotNull] TrMesh mesh, params object[] args);

    public abstract void ImGuiEdit();
}
