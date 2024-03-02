using Hexa.NET.ImGui;
using Silk.NET.OpenGL;
using StbImageSharp;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;
using static StbImageSharp.StbImage;

namespace Triangle.Core.Graphics;

public unsafe class TrTexture : TrGraphics<TrContext>
{
    public const int MaxLevel = 16;

    public TrTexture(TrContext context) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.GenTexture();
        Name = $"Texture {Handle}";
    }

    public string Name { get; set; }

    public uint Width { get; private set; }

    public uint Height { get; private set; }

    public TrPixelFormat PixelFormat { get; private set; }

    public int Anisotropy { get; set; } = 16;

    public TrTextureFilter TextureMinFilter { get; set; } = TrTextureFilter.Linear;

    public TrTextureFilter TextureMagFilter { get; set; } = TrTextureFilter.Linear;

    public TrTextureWrap TextureWrap { get; set; } = TrTextureWrap.Repeat;

    public bool IsGenerateMipmap { get; set; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteTexture(Handle);
    }

    public void Write(string file)
    {
        Name = Path.GetFileName(file);

        stbi__context stbiContext = new(File.OpenRead(file));
        int width;
        int height;
        int comp;
        void* data;
        TrPixelFormat pixelFormat;

        if (stbi__hdr_test(stbiContext) != 0)
        {
            stbi__result_info stbi__result_info = default;
            data = stbi__hdr_load(stbiContext, &width, &height, &comp, (int)ColorComponents.RedGreenBlueAlpha, &stbi__result_info);

            pixelFormat = TrPixelFormat.RGBA16F;
        }
        else
        {
            data = stbi__load_and_postprocess_8bit(stbiContext, &width, &height, &comp, (int)ColorComponents.RedGreenBlueAlpha);

            pixelFormat = TrPixelFormat.RGBA8;
        }

        Write((uint)width, (uint)height, pixelFormat, data);
    }

    public void Write(TrFrame frame)
    {
        byte[] pixels = frame.GetPixels();

        fixed (byte* ptr = pixels)
        {
            Write((uint)frame.Width, (uint)frame.Height, TrPixelFormat.RGBA8, ptr);
        }
    }

    public void Write(uint width, uint height, TrPixelFormat pixelFormat, void* data)
    {
        Width = width;
        Height = height;
        PixelFormat = pixelFormat;

        GL gl = Context.GL;

        (GLEnum Target, GLEnum Format, GLEnum Type) = pixelFormat.ToGL();

        gl.BindTexture(GLEnum.Texture2D, Handle);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)Target, Width, Height, 0, Format, Type, data);
        gl.BindTexture(GLEnum.Texture2D, 0);

        UpdateParameters();
    }

    public void Clear(uint width, uint height, TrPixelFormat pixelFormat)
    {
        Width = width;
        Height = height;
        PixelFormat = pixelFormat;

        GL gl = Context.GL;

        (GLEnum Target, GLEnum Format, GLEnum Type) = pixelFormat.ToGL();

        gl.BindTexture(GLEnum.Texture2D, Handle);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)Target, Width, Height, 0, Format, Type, null);
        gl.BindTexture(GLEnum.Texture2D, 0);
    }

    public void AdjustImGuiProperties()
    {
        ImGui.PushID(GetHashCode());

        ImGui.SeparatorText($"{Name}");

        ImGui.Text($"Size: {Width}x{Height}, Format: {PixelFormat}");

        int anisotropy = Anisotropy;
        ImGui.DragInt("Anisotropy", ref anisotropy, 1, 0, 16);
        Anisotropy = anisotropy;

        TrTextureFilter textureMinFilter = TextureMinFilter;
        ImGuiHelper.EnumCombo("Texture Min Filter", ref textureMinFilter);
        TextureMinFilter = textureMinFilter;

        TrTextureFilter textureMagFilter = TextureMagFilter;
        ImGuiHelper.EnumCombo("Texture Mag Filter", ref textureMagFilter);
        TextureMagFilter = textureMagFilter;

        TrTextureWrap textureWrap = TextureWrap;
        ImGuiHelper.EnumCombo("Texture Wrap", ref textureWrap);
        TextureWrap = textureWrap;

        bool isGenerateMipmap = IsGenerateMipmap;
        ImGui.Checkbox("Generate Mipmap", ref isGenerateMipmap);
        IsGenerateMipmap = isGenerateMipmap;

        ImGuiHelper.Image(this);

        UpdateParameters();

        ImGui.PopID();
    }

    public void UpdateParameters()
    {
        GL gl = Context.GL;

        gl.BindTexture(GLEnum.Texture2D, Handle);

        gl.TexParameter(GLEnum.Texture2D, GLEnum.MaxTextureMaxAnisotropy, Anisotropy);
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.ToGL());
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.ToGL());
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrap.ToGL());
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrap.ToGL());
        gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMaxLevel, IsGenerateMipmap ? MaxLevel : 0);
        gl.GenerateMipmap(GLEnum.Texture2D);

        gl.BindTexture(GLEnum.Texture2D, 0);
    }
}
