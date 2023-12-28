using Silk.NET.OpenGLES;
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

    public void Bind()
    {
        GL gl = Context.GL;

        Frame.Bind();

        gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        gl.ClearDepth(1.0f);
        gl.ClearStencil(0);

        gl.Clear((uint)(GLEnum.ColorBufferBit | GLEnum.DepthBufferBit | GLEnum.StencilBufferBit));
    }

    public void Unbind()
    {
        Frame.Unbind();
    }
}
