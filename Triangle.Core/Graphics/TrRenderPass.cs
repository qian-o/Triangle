using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public class TrRenderPass : TrGraphics<TrContext>
{
    internal TrRenderPass(TrContext context, TrRenderLayer renderLayer, IList<TrRenderPipeline> pipelines) : base(context)
    {
        RenderLayer = renderLayer;
        RenderPipelines = new ReadOnlyCollection<TrRenderPipeline>(pipelines);

        Initialize();
    }

    public TrRenderLayer RenderLayer { get; }

    public ReadOnlyCollection<TrRenderPipeline> RenderPipelines { get; }

    protected override void Initialize()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
