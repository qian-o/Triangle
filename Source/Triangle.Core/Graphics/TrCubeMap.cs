using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrCubeMap : TrGraphics<TrContext>
{
    public TrCubeMap(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.GenTexture();
        Name = $"Cube Texture {Handle}";
    }

    public string Name { get; set; }

    public TrTextureFilter TextureMinFilter { get; set; } = TrTextureFilter.Linear;

    public TrTextureFilter TextureMagFilter { get; set; } = TrTextureFilter.Linear;

    public TrTextureWrap TextureWrap { get; set; } = TrTextureWrap.Repeat;

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteTexture(Handle);
    }

    public void Write(TrTexture texture, TrCubeMapFace cubeMapFace)
    {
        GL gl = Context.GL;

        byte[] pixels = new byte[texture.Width * texture.Height * texture.PixelFormat.Size()];

        fixed (byte* ptr = pixels)
        {
            texture.Read(ptr);

            (GLEnum Target, GLEnum Format, GLEnum Type) = texture.PixelFormat.ToGL();

            gl.BindTexture(GLEnum.TextureCubeMap, Handle);
            gl.TexImage2D(cubeMapFace.ToGL(), 0, (int)Target, texture.Width, texture.Height, 0, Format, Type, ptr);
            gl.BindTexture(GLEnum.TextureCubeMap, 0);

            UpdateParameters();
        }
    }

    public void UpdateParameters()
    {
        GL gl = Context.GL;

        gl.BindTexture(GLEnum.TextureCubeMap, Handle);

        gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureMinFilter, (int)TextureMinFilter.ToGL());
        gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureMagFilter, (int)TextureMagFilter.ToGL());
        gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureWrapS, (int)TextureWrap.ToGL());
        gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureWrapT, (int)TextureWrap.ToGL());
        gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureWrapR, (int)TextureWrap.ToGL());

        gl.BindTexture(GLEnum.TextureCubeMap, 0);
    }
}
