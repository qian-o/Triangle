using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public unsafe class TrDepthFrame : TrGraphics<TrContext>
{
    private uint beforeFrameBuffer;
    private Vector4D<int> beforeViewport;

    public TrDepthFrame(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateFramebuffer();

        DepthBuffer = gl.GenTexture();

        gl.BindTexture(GLEnum.Texture2D, DepthBuffer);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)GLEnum.ClampToEdge);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)GLEnum.ClampToEdge);
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public uint DepthBuffer { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteTexture(DepthBuffer);

        gl.DeleteFramebuffer(Handle);
    }

    public void Update(int width, int height)
    {
        if (Width == width && Height == height)
        {
            return;
        }

        Width = width;
        Height = height;

        GL gl = Context.GL;

        gl.BindTexture(GLEnum.Texture2D, DepthBuffer);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.DepthComponent32, (uint)Width, (uint)Height, 0, GLEnum.DepthComponent, GLEnum.Float, null);
        gl.BindTexture(GLEnum.Texture2D, 0);

        gl.NamedFramebufferTexture(Handle, GLEnum.DepthAttachment, DepthBuffer, 0);

        gl.NamedFramebufferDrawBuffer(Handle, GLEnum.None);
        gl.NamedFramebufferReadBuffer(Handle, GLEnum.None);
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.GetInteger(GLEnum.FramebufferBinding, out int currentFrameBuffer);
        beforeFrameBuffer = (uint)currentFrameBuffer;

        Span<int> viewport = stackalloc int[4];
        gl.GetInteger(GLEnum.Viewport, viewport);
        beforeViewport = new Vector4D<int>(viewport[0], viewport[1], viewport[2], viewport[3]);

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);

        gl.Viewport(0, 0, (uint)Width, (uint)Height);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, beforeFrameBuffer);

        gl.Viewport(beforeViewport.X, beforeViewport.Y, (uint)beforeViewport.Z, (uint)beforeViewport.W);
    }
}
