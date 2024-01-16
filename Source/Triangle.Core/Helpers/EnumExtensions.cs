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
            _ => throw new NotSupportedException("不支持的深度函数。"),
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
            _ => throw new NotSupportedException("不支持的模板函数。"),
        };
    }

    public static GLEnum ToGL(this TrTriangleFace triangleFace)
    {
        return triangleFace switch
        {
            TrTriangleFace.Front => GLEnum.Front,
            TrTriangleFace.Back => GLEnum.Back,
            TrTriangleFace.FrontAndBack => GLEnum.FrontAndBack,
            _ => throw new NotSupportedException("不支持的三角形面。"),
        };
    }

    public static GLEnum ToGL(this TrFrontFaceDirection frontFaceDirection)
    {
        return frontFaceDirection switch
        {
            TrFrontFaceDirection.CounterClockwise => GLEnum.Ccw,
            TrFrontFaceDirection.Clockwise => GLEnum.CW,
            _ => throw new NotSupportedException("不支持的正面方向。"),
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
            _ => throw new NotSupportedException("不支持的混合因子。"),
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
            _ => throw new NotSupportedException("不支持的混合方程。"),
        };
    }

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
