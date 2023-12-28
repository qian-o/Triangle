using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public class TrRenderPass(TrContext context, TrFrame frame, IList<TrRenderPipeline> pipelines) : TrGraphics<TrContext>(context)
{
    public TrFrame Frame { get; } = frame;

    public ReadOnlyCollection<TrRenderPipeline> RenderPipelines { get; } = new ReadOnlyCollection<TrRenderPipeline>(pipelines);

    protected override void Destroy(bool disposing = false)
    {
    }
}
