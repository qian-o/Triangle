using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.EXT;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrTexture : TrGraphics<TrContext>
{
    public TrTexture(TrContext context, TrTextureWrap textureWrap = TrTextureWrap.Repeat, bool isGenerateMipmap = true) : base(context)
    {
        IsGenerateMipmap = isGenerateMipmap;

        GL gl = Context.GL;

        Handle = gl.GenTexture();

        gl.GetFloat((GLEnum)EXT.MaxTextureMaxAnisotropyExt, out float maxAnisotropy);

        gl.BindTexture(GLEnum.Texture2D, Handle);
        gl.TexParameter(GLEnum.Texture2D, (GLEnum)EXT.MaxTextureMaxAnisotropyExt, maxAnisotropy);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)(IsGenerateMipmap ? GLEnum.LinearMipmapLinear : GLEnum.Linear));
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)textureWrap);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)textureWrap);
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public bool IsGenerateMipmap { get; } = true;

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteTexture(Handle);
    }

    public void Write(uint width, uint height, TrPixelFormat pixelFormat, void* data)
    {
        (GLEnum Target, GLEnum Format) = pixelFormat.ToGL();

        GL gl = Context.GL;

        gl.BindTexture(GLEnum.Texture2D, Handle);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)Target, width, height, 0, Format, GLEnum.UnsignedByte, data);
        gl.BindTexture(GLEnum.Texture2D, 0);

        if (IsGenerateMipmap)
        {
            gl.BindTexture(GLEnum.Texture2D, Handle);
            gl.GenerateMipmap(GLEnum.Texture2D);
            gl.BindTexture(GLEnum.Texture2D, 0);
        }
    }
}
