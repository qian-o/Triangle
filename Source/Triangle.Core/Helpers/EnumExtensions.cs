using Silk.NET.OpenGLES;
using Triangle.Core.Enums;

namespace Triangle.Core.Helpers;

public static class EnumExtensions
{
    public static GLEnum ToGL(this TrBufferTarget bufferTarget)
    {
        return bufferTarget switch
        {
            TrBufferTarget.ArrayBuffer => GLEnum.ArrayBuffer,
            TrBufferTarget.ElementArrayBuffer => GLEnum.ElementArrayBuffer,
            TrBufferTarget.PixelPackBuffer => GLEnum.PixelPackBuffer,
            TrBufferTarget.PixelUnpackBuffer => GLEnum.PixelUnpackBuffer,
            TrBufferTarget.UniformBuffer => GLEnum.UniformBuffer,
            TrBufferTarget.TextureBuffer => GLEnum.TextureBuffer,
            TrBufferTarget.Framebuffer => GLEnum.Framebuffer,
            _ => throw new NotSupportedException("不支持的缓冲区目标。"),
        };
    }

    public static GLEnum ToGL(this TrBufferUsage bufferUsage)
    {
        return bufferUsage switch
        {
            TrBufferUsage.Static => GLEnum.StaticDraw,
            TrBufferUsage.Dynamic => GLEnum.DynamicDraw,
            TrBufferUsage.Stream => GLEnum.StreamDraw,
            _ => throw new NotSupportedException("不支持的缓冲区用法。"),
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
            _ => throw new NotSupportedException("不支持的着色器类型。"),
        };
    }
}
