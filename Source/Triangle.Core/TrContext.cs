using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.EXT;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext : TrObject
{
    public TrContext(GL gL)
    {
        GL = gL;

        if (GL.TryGetExtension(out ExtMultisampledRenderToTexture mrt))
        {
            MRT = mrt;
        }
    }

    public GL GL { get; }

    public ExtMultisampledRenderToTexture? MRT { get; }

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            MRT?.Dispose();
        }
    }
}