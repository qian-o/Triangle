using Silk.NET.OpenGLES;
using Triangle.Render.Enums;

namespace Triangle.Render.Helpers;
public static class EnumExtensions
{
    public static (GLEnum Target, GLEnum Format) ToGL(this TrPixelFormat pixelFormat)
    {
        return pixelFormat switch
        {
            TrPixelFormat.R8 => (GLEnum.Red, GLEnum.R8),
            TrPixelFormat.RG8 => (GLEnum.RG, GLEnum.RG8),
            TrPixelFormat.RGB8 => (GLEnum.Rgb, GLEnum.Rgb8),
            TrPixelFormat.RGBA8 => (GLEnum.Rgba, GLEnum.Rgba8),
            _ => throw new NotSupportedException("不支持的像素格式。"),
        };
    }
}

