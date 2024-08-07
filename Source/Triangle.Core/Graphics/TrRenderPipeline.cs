﻿using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Structs;

namespace Triangle.Core.Graphics;

public unsafe class TrRenderPipeline : TrGraphics<TrContext>
{
    public TrRenderPipeline(TrContext context, IList<TrShader> shaders) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateProgram();

        foreach (TrShader shader in shaders)
        {
            gl.AttachShader(Handle, shader.Handle);
        }

        gl.LinkProgram(Handle);

        string error = gl.GetProgramInfoLog(Handle);

        if (!string.IsNullOrEmpty(error))
        {
            gl.DeleteProgram(Handle);

            throw new TrException(error);
        }
    }

    public bool IsColorWrite { get; set; } = true;

    public bool IsDepthTest { get; set; } = true;

    public bool IsDepthWrite { get; set; } = true;

    public TrDepthFunction DepthFunction { get; set; } = TrDepthFunction.Less;

    public bool IsStencilTest { get; set; } = true;

    public bool IsStencilWrite { get; set; } = true;

    public TrStencilFunction StencilFunction { get; set; } = TrStencilFunction.Always;

    public int StencilReference { get; set; }

    public uint StencilMask { get; set; } = 0xFF;

    public bool IsCullFace { get; set; } = true;

    public TrTriangleFace TriangleFace { get; set; } = TrTriangleFace.Back;

    public TrFrontFaceDirection FrontFaceDirection { get; set; } = TrFrontFaceDirection.CounterClockwise;

    public bool IsBlend { get; set; } = true;

    public TrBlendFactor SourceFactor { get; set; } = TrBlendFactor.SrcAlpha;

    public TrBlendFactor DestinationFactor { get; set; } = TrBlendFactor.OneMinusSrcAlpha;

    public TrBlendEquationSeparate BlendEquationSeparate { get; set; } = TrBlendEquationSeparate.Default;

    public TrBlendFuncSeparate BlendFuncSeparate { get; set; } = TrBlendFuncSeparate.Default;

    public bool IsScissorTest { get; set; }

    public bool IsPrimitiveRestart { get; set; }

    public TrPolygon Polygon { get; set; } = TrPolygon.Default;

    public bool IsMultisample { get; set; } = true;

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteProgram(Handle);
    }

    public int GetAttribLocation(string name)
    {
        GL gl = Context.GL;

        return gl.GetAttribLocation(Handle, name);
    }

    public int GetUniformLocation(string name)
    {
        GL gl = Context.GL;

        return gl.GetUniformLocation(Handle, name);
    }

    public void SetRenderLayer(TrRenderLayer renderLayer)
    {
        switch (renderLayer)
        {
            case TrRenderLayer.Background:
                IsColorWrite = true;
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.LessOrEqual;
                IsStencilTest = false;
                IsStencilWrite = false;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = false;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquationSeparate = TrBlendEquationSeparate.Default;
                BlendFuncSeparate = TrBlendFuncSeparate.Default;
                IsScissorTest = false;
                IsPrimitiveRestart = false;
                Polygon = TrPolygon.Default;
                IsMultisample = true;
                break;
            case TrRenderLayer.Geometry:
                IsColorWrite = true;
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = false;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquationSeparate = TrBlendEquationSeparate.Default;
                BlendFuncSeparate = TrBlendFuncSeparate.Default;
                IsScissorTest = false;
                IsPrimitiveRestart = false;
                Polygon = TrPolygon.Default;
                IsMultisample = true;
                break;
            case TrRenderLayer.Opaque:
                IsColorWrite = true;
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = true;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquationSeparate = TrBlendEquationSeparate.Default;
                BlendFuncSeparate = TrBlendFuncSeparate.Default;
                IsScissorTest = false;
                IsPrimitiveRestart = false;
                Polygon = TrPolygon.Default;
                IsMultisample = true;
                break;
            case TrRenderLayer.Transparent:
                IsColorWrite = true;
                IsDepthTest = true;
                IsDepthWrite = false;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = true;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = true;
                SourceFactor = TrBlendFactor.SrcAlpha;
                DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                BlendEquationSeparate = TrBlendEquationSeparate.Default;
                BlendFuncSeparate = TrBlendFuncSeparate.Default;
                IsScissorTest = false;
                IsPrimitiveRestart = false;
                Polygon = TrPolygon.Default;
                IsMultisample = true;
                break;
            case TrRenderLayer.Overlay:
                IsColorWrite = true;
                IsDepthTest = false;
                IsDepthWrite = false;
                DepthFunction = TrDepthFunction.Always;
                IsStencilTest = false;
                IsStencilWrite = false;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = false;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = true;
                SourceFactor = TrBlendFactor.SrcAlpha;
                DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                BlendEquationSeparate = TrBlendEquationSeparate.Default;
                BlendFuncSeparate = TrBlendFuncSeparate.Default;
                IsScissorTest = false;
                IsPrimitiveRestart = false;
                Polygon = TrPolygon.Default;
                IsMultisample = true;
                break;
            default:
                throw new NotSupportedException("不支持的渲染层级。");
        }
    }

    public void BindUniformBlock<T>(uint bindingPoint, TrBuffer<T> ubo) where T : unmanaged
    {
        GL gl = Context.GL;

        gl.BindBufferBase(GLEnum.UniformBuffer, bindingPoint, ubo.Handle);
    }

    public void BindUniformBlock(uint bindingPoint, TrTexture? texture)
    {
        if (texture != null)
        {
            GL gl = Context.GL;

            gl.ActiveTexture(GLEnum.Texture0 + (int)bindingPoint);
            gl.BindTexture(GLEnum.Texture2D, texture.Handle);
        }
    }

    public void BindUniformBlock(uint bindingPoint, TrCubeMap? cubeMap)
    {
        if (cubeMap != null)
        {
            GL gl = Context.GL;

            gl.ActiveTexture(GLEnum.Texture0 + (int)bindingPoint);
            gl.BindTexture(GLEnum.TextureCubeMap, cubeMap.Handle);
        }
    }

    public void BindUniformBlock(uint bindingPoint, TrPixelBuffer? pixelBuffer)
    {
        if (pixelBuffer != null)
        {
            GL gl = Context.GL;

            gl.ActiveTexture(GLEnum.Texture0 + (int)bindingPoint);
            gl.BindTexture(GLEnum.Texture2D, pixelBuffer.Texture.Handle);
        }
    }

    public void BindBufferBlock<T>(uint bindingPoint, TrBuffer<T> ssbo) where T : unmanaged
    {
        GL gl = Context.GL;

        gl.BindBufferBase(GLEnum.ShaderStorageBuffer, bindingPoint, ssbo.Handle);
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.UseProgram(Handle);

        Context.IsColorWrite = IsColorWrite;
        Context.IsDepthTest = IsDepthTest;
        Context.IsDepthWrite = IsDepthWrite;
        Context.DepthFunction = DepthFunction;
        Context.IsStencilTest = IsStencilTest;
        Context.IsStencilWrite = IsStencilWrite;
        Context.StencilFunction = StencilFunction;
        Context.StencilReference = StencilReference;
        Context.StencilMask = StencilMask;
        Context.IsCullFace = IsCullFace;
        Context.TriangleFace = TriangleFace;
        Context.FrontFaceDirection = FrontFaceDirection;
        Context.IsBlend = IsBlend;
        Context.SourceFactor = SourceFactor;
        Context.DestinationFactor = DestinationFactor;
        Context.BlendEquationSeparate = BlendEquationSeparate;
        Context.BlendFuncSeparate = BlendFuncSeparate;
        Context.IsScissorTest = IsScissorTest;
        Context.IsPrimitiveRestart = IsPrimitiveRestart;
        Context.Polygon = Polygon;
        Context.IsMultisample = IsMultisample;
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.UseProgram(0);
    }
}