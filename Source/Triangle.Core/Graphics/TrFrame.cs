using Silk.NET.OpenGL;
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
            MultisampleDepthStencilBuffer = 0;
        }
        else
        {
            Handle = gl.GenFramebuffer();
            ColorBuffer = gl.GenTexture();
            DepthStencilBuffer = gl.GenRenderbuffer();
            MultisampleFrameBuffer = gl.GenFramebuffer();
            MultisampleColorBuffer = gl.GenTexture();
            MultisampleDepthStencilBuffer = gl.GenRenderbuffer();

            gl.BindTexture(GLEnum.Texture2D, ColorBuffer);

            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Nearest);
            gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Nearest);

            gl.BindTexture(GLEnum.Texture2D, 0);
        }
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public uint Samples { get; }

    public uint ColorBuffer { get; }

    public uint DepthStencilBuffer { get; }

    public uint MultisampleFrameBuffer { get; }

    public uint MultisampleColorBuffer { get; }

    public uint MultisampleDepthStencilBuffer { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteFramebuffer(Handle);
        gl.DeleteTexture(ColorBuffer);
        gl.DeleteRenderbuffer(DepthStencilBuffer);
        gl.DeleteFramebuffer(MultisampleFrameBuffer);
        gl.DeleteTexture(MultisampleColorBuffer);
        gl.DeleteRenderbuffer(MultisampleDepthStencilBuffer);
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

        // 最终渲染使用的帧缓冲区
        {
            gl.BindTexture(GLEnum.Texture2D, ColorBuffer);
            gl.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgb, (uint)Width, (uint)Height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
            gl.BindTexture(GLEnum.Texture2D, 0);

            gl.BindRenderbuffer(GLEnum.Renderbuffer, DepthStencilBuffer);
            gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
            gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

            gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
            gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, ColorBuffer, 0);
            gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, DepthStencilBuffer);
            gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        }

        // 多重采样的帧缓冲区
        {
            gl.BindTexture(GLEnum.Texture2DMultisample, MultisampleColorBuffer);
            gl.TexImage2DMultisample(GLEnum.Texture2DMultisample, Samples, GLEnum.Rgb, (uint)Width, (uint)Height, true);
            gl.BindTexture(GLEnum.Texture2DMultisample, 0);

            gl.BindRenderbuffer(GLEnum.Renderbuffer, MultisampleDepthStencilBuffer);
            gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, Samples, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
            gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

            gl.BindFramebuffer(GLEnum.Framebuffer, MultisampleFrameBuffer);
            gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2DMultisample, MultisampleColorBuffer, 0);
            gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, MultisampleDepthStencilBuffer);
            gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        }
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, MultisampleFrameBuffer);
        gl.Viewport(0, 0, (uint)Width, (uint)Height);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        gl.BindFramebuffer(GLEnum.ReadFramebuffer, MultisampleFrameBuffer);
        gl.BindFramebuffer(GLEnum.DrawFramebuffer, Handle);
        gl.BlitFramebuffer(0, 0, Width, Height, 0, 0, Width, Height, (uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit, GLEnum.Nearest);
        gl.BindFramebuffer(GLEnum.DrawFramebuffer, 0);
        gl.BindFramebuffer(GLEnum.ReadFramebuffer, 0);
    }
}
