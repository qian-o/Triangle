using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public class TrRenderPass(TrContext context, TrRenderLayer renderLayer, IList<TrRenderPipeline> pipelines) : TrGraphics<TrContext>(context)
{
    public TrRenderLayer RenderLayer { get; } = renderLayer;

    public ReadOnlyCollection<TrRenderPipeline> RenderPipelines { get; } = new ReadOnlyCollection<TrRenderPipeline>(pipelines);

    protected override void Destroy(bool disposing = false)
    {
    }
}
