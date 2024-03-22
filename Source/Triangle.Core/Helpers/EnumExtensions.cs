using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Enums;

namespace Triangle.Core.Helpers;

public static class EnumExtensions
{
    public static GLEnum ToGL(this TrDepthFunction depthFunction)
    {
        return depthFunction switch
        {
            TrDepthFunction.Never => GLEnum.Never,
            TrDepthFunction.Less => GLEnum.Less,
            TrDepthFunction.Equal => GLEnum.Equal,
            TrDepthFunction.LessOrEqual => GLEnum.Lequal,
            TrDepthFunction.Greater => GLEnum.Greater,
            TrDepthFunction.NotEqual => GLEnum.Notequal,
            TrDepthFunction.GreaterOrEqual => GLEnum.Gequal,
            TrDepthFunction.Always => GLEnum.Always,
            _ => throw new NotSupportedException("不支持的深度函数。")
        };
    }

    public static GLEnum ToGL(this TrStencilFunction stencilFunction)
    {
        return stencilFunction switch
        {
            TrStencilFunction.Never => GLEnum.Never,
            TrStencilFunction.Less => GLEnum.Less,
            TrStencilFunction.Equal => GLEnum.Equal,
            TrStencilFunction.LessOrEqual => GLEnum.Lequal,
            TrStencilFunction.Greater => GLEnum.Greater,
            TrStencilFunction.NotEqual => GLEnum.Notequal,
            TrStencilFunction.GreaterOrEqual => GLEnum.Gequal,
            TrStencilFunction.Always => GLEnum.Always,
            _ => throw new NotSupportedException("不支持的模板函数。")
        };
    }

    public static GLEnum ToGL(this TrTriangleFace triangleFace)
    {
        return triangleFace switch
        {
            TrTriangleFace.Front => GLEnum.Front,
            TrTriangleFace.Back => GLEnum.Back,
            TrTriangleFace.FrontAndBack => GLEnum.FrontAndBack,
            _ => throw new NotSupportedException("不支持的三角形面。")
        };
    }

    public static GLEnum ToGL(this TrFrontFaceDirection frontFaceDirection)
    {
        return frontFaceDirection switch
        {
            TrFrontFaceDirection.CounterClockwise => GLEnum.Ccw,
            TrFrontFaceDirection.Clockwise => GLEnum.CW,
            _ => throw new NotSupportedException("不支持的正面方向。")
        };
    }

    public static GLEnum ToGL(this TrBlendFactor blendFactor)
    {
        return blendFactor switch
        {
            TrBlendFactor.Zero => GLEnum.Zero,
            TrBlendFactor.One => GLEnum.One,
            TrBlendFactor.SrcColor => GLEnum.SrcColor,
            TrBlendFactor.OneMinusSrcColor => GLEnum.OneMinusSrcColor,
            TrBlendFactor.DstColor => GLEnum.DstColor,
            TrBlendFactor.OneMinusDstColor => GLEnum.OneMinusDstColor,
            TrBlendFactor.SrcAlpha => GLEnum.SrcAlpha,
            TrBlendFactor.OneMinusSrcAlpha => GLEnum.OneMinusSrcAlpha,
            TrBlendFactor.DstAlpha => GLEnum.DstAlpha,
            TrBlendFactor.OneMinusDstAlpha => GLEnum.OneMinusDstAlpha,
            TrBlendFactor.ConstantColor => GLEnum.ConstantColor,
            TrBlendFactor.OneMinusConstantColor => GLEnum.OneMinusConstantColor,
            TrBlendFactor.ConstantAlpha => GLEnum.ConstantAlpha,
            TrBlendFactor.OneMinusConstantAlpha => GLEnum.OneMinusConstantAlpha,
            TrBlendFactor.SrcAlphaSaturate => GLEnum.SrcAlphaSaturate,
            _ => throw new NotSupportedException("不支持的混合因子。")
        };
    }

    public static GLEnum ToGL(this TrBlendEquation blendEquation)
    {
        return blendEquation switch
        {
            TrBlendEquation.Add => GLEnum.FuncAdd,
            TrBlendEquation.Subtract => GLEnum.FuncSubtract,
            TrBlendEquation.ReverseSubtract => GLEnum.FuncReverseSubtract,
            TrBlendEquation.Min => GLEnum.Min,
            TrBlendEquation.Max => GLEnum.Max,
            _ => throw new NotSupportedException("不支持的混合方程。")
        };
    }

    public static GLEnum ToGL(this TrShaderType shaderType)
    {
        return shaderType switch
        {
            TrShaderType.Vertex => GLEnum.VertexShader,
            TrShaderType.Geometry => GLEnum.GeometryShader,
            TrShaderType.Fragment => GLEnum.FragmentShader,
            TrShaderType.Compute => GLEnum.ComputeShader,
            _ => throw new NotSupportedException("不支持的着色器类型。")
        };
    }

    public static GLEnum ToGL(this TrTextureWrap textureWrap)
    {
        return textureWrap switch
        {
            TrTextureWrap.Repeat => GLEnum.Repeat,
            TrTextureWrap.MirroredRepeat => GLEnum.MirroredRepeat,
            TrTextureWrap.ClampToEdge => GLEnum.ClampToEdge,
            TrTextureWrap.ClampToBorder => GLEnum.ClampToBorder,
            _ => throw new NotSupportedException("不支持的纹理包裹。")
        };
    }

    public static GLEnum ToGL(this TrTextureFilter textureFilter)
    {
        return textureFilter switch
        {
            TrTextureFilter.Nearest => GLEnum.Nearest,
            TrTextureFilter.Linear => GLEnum.Linear,
            TrTextureFilter.NearestMipmapNearest => GLEnum.NearestMipmapNearest,
            TrTextureFilter.LinearMipmapNearest => GLEnum.LinearMipmapNearest,
            TrTextureFilter.NearestMipmapLinear => GLEnum.NearestMipmapLinear,
            TrTextureFilter.LinearMipmapLinear => GLEnum.LinearMipmapLinear,
            _ => throw new NotSupportedException("不支持的纹理过滤。")
        };
    }

    public static (GLEnum Target, GLEnum Format, GLEnum Type) ToGL(this TrPixelFormat pixelFormat)
    {
        return pixelFormat switch
        {
            TrPixelFormat.R8 => (GLEnum.R8, GLEnum.Red, GLEnum.UnsignedByte),
            TrPixelFormat.RG8 => (GLEnum.RG8, GLEnum.RG, GLEnum.UnsignedByte),
            TrPixelFormat.RGB8 => (GLEnum.Rgb8, GLEnum.Rgb, GLEnum.UnsignedByte),
            TrPixelFormat.RGBA8 => (GLEnum.Rgba8, GLEnum.Rgba, GLEnum.UnsignedByte),
            TrPixelFormat.R16 => (GLEnum.R16, GLEnum.Red, GLEnum.UnsignedShort),
            TrPixelFormat.RG16 => (GLEnum.RG16, GLEnum.RG, GLEnum.UnsignedShort),
            TrPixelFormat.RGB16 => (GLEnum.Rgb16, GLEnum.Rgb, GLEnum.UnsignedShort),
            TrPixelFormat.RGBA16 => (GLEnum.Rgba16, GLEnum.Rgba, GLEnum.UnsignedShort),
            TrPixelFormat.R16F => (GLEnum.R16f, GLEnum.Red, GLEnum.Float),
            TrPixelFormat.RG16F => (GLEnum.RG16f, GLEnum.RG, GLEnum.Float),
            TrPixelFormat.RGB16F => (GLEnum.Rgb16f, GLEnum.Rgb, GLEnum.Float),
            TrPixelFormat.RGBA16F => (GLEnum.Rgba16f, GLEnum.Rgba, GLEnum.Float),
            TrPixelFormat.DepthComponent16 => (GLEnum.DepthComponent16, GLEnum.DepthComponent, GLEnum.UnsignedShort),
            TrPixelFormat.DepthComponent24 => (GLEnum.DepthComponent24, GLEnum.DepthComponent, GLEnum.UnsignedInt),
            TrPixelFormat.DepthComponent32 => (GLEnum.DepthComponent32, GLEnum.DepthComponent, GLEnum.UnsignedInt),
            TrPixelFormat.DepthComponent32F => (GLEnum.DepthComponent32f, GLEnum.DepthComponent, GLEnum.Float),
            _ => throw new NotSupportedException("不支持的像素格式。")
        };
    }

    public static GLEnum ToGL(this TrCubeMapFace textureCubeMap)
    {
        return textureCubeMap switch
        {
            TrCubeMapFace.PositiveX => GLEnum.TextureCubeMapPositiveX,
            TrCubeMapFace.NegativeX => GLEnum.TextureCubeMapNegativeX,
            TrCubeMapFace.PositiveY => GLEnum.TextureCubeMapPositiveY,
            TrCubeMapFace.NegativeY => GLEnum.TextureCubeMapNegativeY,
            TrCubeMapFace.PositiveZ => GLEnum.TextureCubeMapPositiveZ,
            TrCubeMapFace.NegativeZ => GLEnum.TextureCubeMapNegativeZ,
            _ => throw new NotSupportedException("不支持的立方体贴图。")
        };
    }

    public static GLEnum ToGL(this TrPolygonMode polygonMode)
    {
        return polygonMode switch
        {
            TrPolygonMode.Fill => GLEnum.Fill,
            TrPolygonMode.Line => GLEnum.Line,
            TrPolygonMode.Point => GLEnum.Point,
            _ => throw new NotSupportedException("不支持的多边形模式。")
        };
    }

    public static int Size(this TrPixelFormat pixelFormat)
    {
        return pixelFormat switch
        {
            TrPixelFormat.R8 => sizeof(byte),
            TrPixelFormat.RG8 => sizeof(byte) * 2,
            TrPixelFormat.RGB8 => sizeof(byte) * 3,
            TrPixelFormat.RGBA8 => sizeof(byte) * 4,
            TrPixelFormat.R16 => sizeof(ushort),
            TrPixelFormat.RG16 => sizeof(ushort) * 2,
            TrPixelFormat.RGB16 => sizeof(ushort) * 3,
            TrPixelFormat.RGBA16 => sizeof(ushort) * 4,
            TrPixelFormat.R16F => sizeof(float),
            TrPixelFormat.RG16F => sizeof(float) * 2,
            TrPixelFormat.RGB16F => sizeof(float) * 3,
            TrPixelFormat.RGBA16F => sizeof(float) * 4,
            _ => throw new NotSupportedException("不支持的像素格式。")
        };
    }

    public static float Alignment(this TrHorizontalAlignment alignment, Vector2D<float> area, Vector2D<float> size)
    {
        return alignment switch
        {
            TrHorizontalAlignment.Left => 0.0f,
            TrHorizontalAlignment.Center => (area.X - size.X) / 2.0f,
            TrHorizontalAlignment.Right => area.X - size.X,
            TrHorizontalAlignment.Stretch => 0.0f,
            _ => throw new NotSupportedException("不支持的水平对齐。")
        };
    }

    public static float Alignment(this TrVerticalAlignment alignment, Vector2D<float> area, Vector2D<float> size)
    {
        return alignment switch
        {
            TrVerticalAlignment.Top => 0.0f,
            TrVerticalAlignment.Center => (area.Y - size.Y) / 2.0f,
            TrVerticalAlignment.Bottom => area.Y - size.Y,
            TrVerticalAlignment.Stretch => 0.0f,
            _ => throw new NotSupportedException("不支持的垂直对齐。")
        };
    }
}
