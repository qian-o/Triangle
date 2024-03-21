using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrPixelBuffer : TrGraphics<TrContext>
{
    public TrPixelBuffer(TrContext context, int width, int height, TrPixelFormat pixelFormat) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateBuffer();
        Width = width;
        Height = height;
        PixelFormat = pixelFormat;
        Texture = new TrTexture(Context);

        Texture.Clear((uint)Width, (uint)Height, PixelFormat);

        gl.NamedBufferStorage(Handle, (uint)(Width * Height * PixelFormat.Size()), null, (uint)GLEnum.DynamicStorageBit);
    }

    public int Width { get; }

    public int Height { get; }

    public TrPixelFormat PixelFormat { get; }

    public TrTexture Texture { get; }

    public void Update<T>(int width, int height, T[] data) where T : unmanaged
    {
        if (Width < width || Height < height)
        {
            throw new ArgumentException("Invalid size.");
        }

        GL gl = Context.GL;

        fixed (T* dataPtr = data)
        {
            gl.NamedBufferSubData(Handle, 0, (uint)(data.Length * sizeof(T)), dataPtr);
        }

        Texture.SubWrite(0, 0, (uint)width, (uint)height, PixelFormat, this);
    }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);

        Texture.Dispose();
    }
}
