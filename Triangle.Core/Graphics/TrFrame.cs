using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.EXT;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public unsafe class TrFrame : TrGraphics<TrContext>
{
    public TrFrame(TrContext context, bool @default = false, uint samples = 1) : base(context)
    {
        Samples = samples;

        GL gl = Context.GL;

        if (@default)
        {
            Handle = 0;
            ColorBuffer = 0;
            DepthStencilBuffer = 0;
        }
        else
        {
            Handle = gl.GenFramebuffer();
            ColorBuffer = gl.GenTexture();
            DepthStencilBuffer = gl.GenRenderbuffer();

            gl.BindTexture(GLEnum.Texture2D, ColorBuffer);

            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);

            gl.BindTexture(GLEnum.Texture2D, 0);
        }
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public uint Samples { get; }

    public uint ColorBuffer { get; }

    public uint DepthStencilBuffer { get; }

    protected override void Destroy(bool disposing = false)
    {
    }

    public void Update(int width, int height)
    {
        if (Handle == 0)
        {
            return;
        }

        if (Width == width && Height == height)
        {
            return;
        }

        GL gl = Context.GL;
        ExtMultisampledRenderToTexture? mrt = Context.MRT;

        Width = width;
        Height = height;

        gl.BindTexture(GLEnum.Texture2D, ColorBuffer);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgb, (uint)Width, (uint)Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
        gl.BindTexture(GLEnum.Texture2D, 0);

        gl.BindRenderbuffer(GLEnum.Renderbuffer, DepthStencilBuffer);
        if (mrt != null)
        {
            mrt.RenderbufferStorageMultisample((EXT)GLEnum.Renderbuffer, Samples, (EXT)GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
        }
        else
        {
            gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
        }
        gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        if (mrt != null)
        {
            mrt.FramebufferTexture2DMultisample((EXT)GLEnum.Framebuffer, (EXT)GLEnum.ColorAttachment0, (EXT)GLEnum.Texture2D, ColorBuffer, 0, Samples);
        }
        else
        {
            gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, ColorBuffer, 0);
        }
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, DepthStencilBuffer);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        gl.Viewport(0, 0, (uint)Width, (uint)Height);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, 0);
    }
}
