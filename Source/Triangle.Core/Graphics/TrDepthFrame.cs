using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public unsafe class TrDepthFrame : TrGraphics<TrContext>
{
    private uint beforeFrameBuffer;
    private Vector4D<int> beforeViewport;

    public TrDepthFrame(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateFramebuffer();

        Texture = new TrTexture(Context)
        {
            TextureMinFilter = TrTextureFilter.Linear,
            TextureMagFilter = TrTextureFilter.Linear,
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        Texture.UpdateParameters();
    }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public TrTexture Texture { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteFramebuffer(Handle);

        Texture.Dispose();
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

        Texture.Clear((uint)Width, (uint)Height, TrPixelFormat.DepthComponent32F);

        gl.NamedFramebufferTexture(Handle, GLEnum.DepthAttachment, Texture.Handle, 0);

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
