using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

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

    public TrTextureFilter TextureMinFilter { get; set; } = TrTextureFilter.NearestMipmapLinear;

    public TrTextureFilter TextureMagFilter { get; set; } = TrTextureFilter.LinearMipmapLinear;

    public TrTextureWrap TextureWrap { get; set; } = TrTextureWrap.Repeat;

    public bool IsGenerateMipmap { get; set; } = true;

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteTexture(Handle);
    }

    public void WriteImage(string file)
    {
        Name = Path.GetFileName(file);

        ImageResult image = ImageResult.FromMemory(File.ReadAllBytes(file), ColorComponents.RedGreenBlueAlpha);

        fixed (byte* ptr = image.Data)
        {
            Write((uint)image.Width, (uint)image.Height, TrPixelFormat.RGBA8, ptr);
        }
    }

    public void Write(uint width, uint height, TrPixelFormat pixelFormat, void* data)
    {
        Width = width;
        Height = height;
        PixelFormat = pixelFormat;

        GL gl = Context.GL;

        (GLEnum Target, GLEnum Format) = pixelFormat.ToGL();

        gl.BindTexture(GLEnum.Texture2D, Handle);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)Target, Width, Height, 0, Format, GLEnum.UnsignedByte, data);
        gl.BindTexture(GLEnum.Texture2D, 0);

        UpdateParameters();
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

        ImGuiHelper.Image(this, new Vector2D<float>(256.0f, 256.0f));

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
