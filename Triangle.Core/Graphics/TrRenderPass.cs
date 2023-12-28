using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public class TrRenderPass(TrContext context, IList<TrRenderPipeline> pipelines) : TrGraphics<TrContext>(context)
{
    public ReadOnlyCollection<TrRenderPipeline> RenderPipelines { get; } = new ReadOnlyCollection<TrRenderPipeline>(pipelines);

    protected override void Destroy(bool disposing = false)
    {
    }
}
