using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrFrame : TrGraphics<TrContext>
{
    private uint beforeFrameBuffer;

    public TrFrame(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateFramebuffer();

        Framebuffer = gl.CreateFramebuffer();
        ColorBuffer = gl.CreateRenderbuffer();
        DepthStencilBuffer = gl.CreateRenderbuffer();

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

        GL gl = Context.GL;

        Texture.Clear((uint)Width, (uint)Height, PixelFormat);

        gl.NamedFramebufferTexture(Handle, GLEnum.ColorAttachment0, Texture.Handle, 0);

        // 多重采样缓冲区
        {
            gl.NamedRenderbufferStorageMultisample(ColorBuffer, (uint)Samples, PixelFormat.ToGL().Target, (uint)Width, (uint)Height);
            gl.NamedRenderbufferStorageMultisample(DepthStencilBuffer, (uint)Samples, GLEnum.Depth32fStencil8, (uint)Width, (uint)Height);

            gl.NamedFramebufferRenderbuffer(Framebuffer, GLEnum.ColorAttachment0, GLEnum.Renderbuffer, ColorBuffer);
            gl.NamedFramebufferRenderbuffer(Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, DepthStencilBuffer);
        }
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.GetInteger(GLEnum.FramebufferBinding, out int currentFrameBuffer);
        beforeFrameBuffer = (uint)currentFrameBuffer;

        gl.BindFramebuffer(GLEnum.Framebuffer, Framebuffer);

        gl.Viewport(0, 0, (uint)Width, (uint)Height);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.BindFramebuffer(GLEnum.Framebuffer, beforeFrameBuffer);

        gl.BlitNamedFramebuffer(Framebuffer, Handle, 0, 0, Width, Height, 0, 0, Width, Height, (uint)GLEnum.ColorBufferBit, GLEnum.Nearest);
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

        byte[] pixels = new byte[Width * Height * PixelFormat.Size()];

        fixed (byte* ptr = pixels)
        {
            (GLEnum _, GLEnum Format, GLEnum Type) = PixelFormat.ToGL();

            gl.BindFramebuffer(GLEnum.Framebuffer, Handle);
            gl.ReadBuffer(GLEnum.ColorAttachment0);
            gl.ReadPixels(0, 0, (uint)Width, (uint)Height, Format, Type, ptr);
            gl.BindFramebuffer(GLEnum.Framebuffer, 0);
        }

        return pixels;
    }
}
