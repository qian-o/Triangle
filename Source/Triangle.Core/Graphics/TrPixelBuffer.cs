using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public class TrPixelBuffer : TrGraphics<TrContext>
{
    public TrPixelBuffer(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateBuffer();
    }

    public void Initialize(uint width, uint height, TrPixelFormat pixelFormat)
    {

    }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);
    }
}
