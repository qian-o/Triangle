using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public unsafe class TrFrame : TrGraphics<TrContext>
{
    public TrFrame(TrContext context, uint samples = 1) : base(context)
    {
        Samples = samples;

        GL gl = Context.GL;

        Handle = gl.GenFramebuffer();
        Texture = gl.GenTexture();

        Framebuffer = gl.GenFramebuffer();
        ColorBuffer = gl.GenRenderbuffer();
        DepthStencilBuffer = gl.GenRenderbuffer();

        gl.BindTexture(GLEnum.Texture2D, Texture);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public uint Samples { get; }

    public uint Texture { get; }

    public uint Framebuffer { get; }

    public uint ColorBuffer { get; }

    public uint DepthStencilBuffer { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteFramebuffer(Handle);
        gl.DeleteTexture(Texture);

        gl.DeleteFramebuffer(Framebuffer);
        gl.DeleteRenderbuffer(ColorBuffer);
        gl.DeleteRenderbuffer(DepthStencilBuffer);
    }

    public void Update(int width, int height)
    {
        if (Width == width && Height == height)
        {
            return;
        }

        Width = width;
        Height = height;

        if (Handle == 0)
        {
            return;
        }

        GL gl = Context.GL;

        gl.BindTexture(GLEnum.Texture2D, Texture);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgb, (uint)Width, (uint)Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
        gl.BindTexture(GLEnum.Texture2D, 0);

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, Texture, 0);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        // 多重采样缓冲区
        {
            gl.BindRenderbuffer(GLEnum.Renderbuffer, ColorBuffer);
            gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, Samples, GLEnum.Rgb8, (uint)Width, (uint)Height);
            gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

            gl.BindRenderbuffer(GLEnum.Renderbuffer, DepthStencilBuffer);
            gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, Samples, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
            gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

            gl.BindFramebuffer(GLEnum.Framebuffer, Framebuffer);
            gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Renderbuffer, ColorBuffer);
            gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, DepthStencilBuffer);
            gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        }
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, Framebuffer);
        gl.Viewport(0, 0, (uint)Width, (uint)Height);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        gl.BindFramebuffer(GLEnum.ReadFramebuffer, Framebuffer);
        gl.BindFramebuffer(GLEnum.DrawFramebuffer, Handle);
        gl.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, (uint)GLEnum.ColorBufferBit, GLEnum.Nearest);
        gl.BindFramebuffer(GLEnum.DrawFramebuffer, 0);
        gl.BindFramebuffer(GLEnum.ReadFramebuffer, 0);
    }
}
