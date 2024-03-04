using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrFrame : TrGraphics<TrContext>
{
    public TrFrame(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.GenFramebuffer();

        Framebuffer = gl.GenFramebuffer();
        ColorBuffer = gl.GenRenderbuffer();
        DepthStencilBuffer = gl.GenRenderbuffer();

        Texture = new TrTexture(Context)
        {
            TextureMinFilter = TrTextureFilter.Nearest,
            TextureMagFilter = TrTextureFilter.Nearest,
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        Texture.UpdateParameters();
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Samples { get; private set; }

    public TrPixelFormat PixelFormat { get; private set; }

    public TrTexture Texture { get; }

    public uint Framebuffer { get; }

    public uint ColorBuffer { get; }

    public uint DepthStencilBuffer { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteFramebuffer(Handle);

        gl.DeleteFramebuffer(Framebuffer);
        gl.DeleteRenderbuffer(ColorBuffer);
        gl.DeleteRenderbuffer(DepthStencilBuffer);

        Texture.Dispose();
    }

    public void Update(int width, int height, int samples = 1, TrPixelFormat pixelFormat = TrPixelFormat.RGB8)
    {
        if (samples < 1)
        {
            throw new TrException("The number of samples must be greater than or equal to 1.");
        }

        if (Width == width && Height == height && Samples == samples && PixelFormat == pixelFormat)
        {
            return;
        }

        Width = width;
        Height = height;
        Samples = samples;
        PixelFormat = pixelFormat;

        if (Handle == 0)
        {
            return;
        }

        GL gl = Context.GL;

        Texture.Clear((uint)Width, (uint)Height, PixelFormat);

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, Texture.Handle, 0);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        // 多重采样缓冲区
        {
            gl.BindRenderbuffer(GLEnum.Renderbuffer, ColorBuffer);
            gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, (uint)Samples, PixelFormat.ToGL().Target, (uint)Width, (uint)Height);
            gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);

            gl.BindRenderbuffer(GLEnum.Renderbuffer, DepthStencilBuffer);
            gl.RenderbufferStorageMultisample(GLEnum.Renderbuffer, (uint)Samples, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);
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

    public Vector4D<byte> GetPixel(int x, int y)
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        gl.ReadBuffer(GLEnum.ColorAttachment0);

        Vector4D<byte> pixel = new();
        gl.ReadPixels(x, Height - y, 1, 1, GLEnum.Rgba, GLEnum.UnsignedByte, &pixel);

        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        return pixel;
    }

    public byte[] GetPixels()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
        gl.ReadBuffer(GLEnum.ColorAttachment0);

        byte[] pixels = new byte[Width * Height * 4];
        gl.ReadPixels<byte>(0, 0, (uint)Width, (uint)Height, GLEnum.Rgba, GLEnum.UnsignedByte, pixels);

        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        return pixels;
    }
}
