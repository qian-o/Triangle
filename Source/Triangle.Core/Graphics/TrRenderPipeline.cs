using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;
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

    public TrBlendEquation BlendEquation { get; set; } = TrBlendEquation.Add;

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
                BlendEquation = TrBlendEquation.Add;
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
                BlendEquation = TrBlendEquation.Add;
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
                BlendEquation = TrBlendEquation.Add;
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
                BlendEquation = TrBlendEquation.Add;
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
                BlendEquation = TrBlendEquation.Add;
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
        GL gl = Context.GL;

        gl.ActiveTexture(GLEnum.Texture0 + (int)bindingPoint);
        gl.BindTexture(GLEnum.Texture2D, texture != null ? texture.Handle : 0);
    }

    public void BindUniformBlock(uint bindingPoint, TrCubeMap? cubeMap)
    {
        GL gl = Context.GL;

        gl.ActiveTexture(GLEnum.Texture0 + (int)bindingPoint);
        gl.BindTexture(GLEnum.TextureCubeMap, cubeMap != null ? cubeMap.Handle : 0);
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.UseProgram(Handle);

        gl.ColorMask(IsColorWrite, IsColorWrite, IsColorWrite, IsColorWrite);

        if (IsDepthTest)
        {
            gl.Enable(GLEnum.DepthTest);
        }
        else
        {
            gl.Disable(GLEnum.DepthTest);
        }

        gl.DepthMask(IsDepthWrite);

        gl.DepthFunc(DepthFunction.ToGL());

        if (IsStencilTest)
        {
            gl.Enable(GLEnum.StencilTest);
        }
        else
        {
            gl.Disable(GLEnum.StencilTest);
        }

        gl.StencilMask((uint)(IsStencilWrite ? 0xFF : 0x00));

        gl.StencilFunc(StencilFunction.ToGL(), StencilReference, StencilMask);

        gl.StencilOp(GLEnum.Keep, GLEnum.Keep, GLEnum.Keep);

        if (IsCullFace)
        {
            gl.Enable(GLEnum.CullFace);
        }
        else
        {
            gl.Disable(GLEnum.CullFace);
        }

        gl.CullFace(TriangleFace.ToGL());
        gl.FrontFace(FrontFaceDirection.ToGL());

        if (IsBlend)
        {
            gl.Enable(GLEnum.Blend);
        }
        else
        {
            gl.Disable(GLEnum.Blend);
        }

        gl.BlendFunc(SourceFactor.ToGL(), DestinationFactor.ToGL());

        gl.BlendEquation(BlendEquation.ToGL());

        gl.BlendFuncSeparate(BlendFuncSeparate.SrcRGB.ToGL(), BlendFuncSeparate.DstRGB.ToGL(), BlendFuncSeparate.SrcAlpha.ToGL(), BlendFuncSeparate.DstAlpha.ToGL());

        if (IsScissorTest)
        {
            gl.Enable(GLEnum.ScissorTest);
        }
        else
        {
            gl.Disable(GLEnum.ScissorTest);
        }

        if (IsPrimitiveRestart)
        {
            gl.Enable(GLEnum.PrimitiveRestart);
        }
        else
        {
            gl.Disable(GLEnum.PrimitiveRestart);
        }

        gl.PolygonMode(Polygon.Face.ToGL(), Polygon.Mode.ToGL());
        gl.LineWidth(Polygon.LineWidth);
        gl.PointSize(Polygon.PointSize);

        if (IsMultisample)
        {
            gl.Enable(GLEnum.Multisample);
        }
        else
        {
            gl.Disable(GLEnum.Multisample);
        }

        // Other settings, not controlled by the render pipeline.
        gl.Enable(GLEnum.TextureCubeMapSeamless);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.UseProgram(0);
    }
}