using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Graphics;

public unsafe class TrRenderPipeline : TrGraphics<TrContext>
{
    public TrRenderPipeline(TrContext context, [NotNull] IList<TrShader> shaders) : base(context)
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

    public bool IsDepthTest { get; set; } = true;

    public bool IsDepthWrite { get; set; } = true;

    public TrDepthFunction DepthFunction { get; set; } = TrDepthFunction.Less;

    public bool IsStencilTest { get; set; } = true;

    public uint StencilMask { get; set; } = 0xFF;

    public TrStencilFunction StencilFunction { get; set; } = TrStencilFunction.Always;

    public int StencilReference { get; set; }

    public bool IsStencilWrite { get; set; } = true;

    public bool IsCullFace { get; set; } = true;

    public TrCullFaceMode CullFaceMode { get; set; } = TrCullFaceMode.CounterClockwise;

    public bool IsBlend { get; set; } = true;

    public TrBlendFactor SourceFactor { get; set; } = TrBlendFactor.SrcAlpha;

    public TrBlendFactor DestinationFactor { get; set; } = TrBlendFactor.OneMinusSrcAlpha;

    public TrBlendEquation BlendEquation { get; set; } = TrBlendEquation.Add;

    public bool IsColorWrite { get; set; } = true;

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
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.LessOrEqual;
                IsStencilTest = false;
                IsStencilWrite = false;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = false;
                CullFaceMode = TrCullFaceMode.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquation = TrBlendEquation.Add;
                IsColorWrite = true;
                break;
            case TrRenderLayer.Geometry:
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = true;
                CullFaceMode = TrCullFaceMode.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquation = TrBlendEquation.Add;
                IsColorWrite = true;
                break;
            case TrRenderLayer.Opaque:
                IsDepthTest = true;
                IsDepthWrite = true;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = true;
                CullFaceMode = TrCullFaceMode.CounterClockwise;
                IsBlend = false;
                SourceFactor = TrBlendFactor.One;
                DestinationFactor = TrBlendFactor.Zero;
                BlendEquation = TrBlendEquation.Add;
                IsColorWrite = true;
                break;
            case TrRenderLayer.Transparent:
                IsDepthTest = true;
                IsDepthWrite = false;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = true;
                IsStencilWrite = true;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = true;
                CullFaceMode = TrCullFaceMode.CounterClockwise;
                IsBlend = true;
                SourceFactor = TrBlendFactor.SrcAlpha;
                DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                BlendEquation = TrBlendEquation.Add;
                IsColorWrite = true;
                break;
            case TrRenderLayer.Overlay:
                IsDepthTest = false;
                IsDepthWrite = false;
                DepthFunction = TrDepthFunction.Less;
                IsStencilTest = false;
                IsStencilWrite = false;
                StencilFunction = TrStencilFunction.Always;
                StencilReference = 0;
                StencilMask = 0xFF;
                IsCullFace = false;
                CullFaceMode = TrCullFaceMode.CounterClockwise;
                IsBlend = true;
                SourceFactor = TrBlendFactor.SrcAlpha;
                DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                BlendEquation = TrBlendEquation.Add;
                IsColorWrite = true;
                break;
            default:
                throw new NotSupportedException("不支持的渲染层级。");
        }
    }

    public void SetUniform(string name, int value)
    {
        GL gl = Context.GL;

        gl.Uniform1(GetUniformLocation(name), value);
    }

    public void SetUniform(string name, float value)
    {
        GL gl = Context.GL;

        gl.Uniform1(GetUniformLocation(name), value);
    }

    public void SetUniform(string name, Vector2D<float> value)
    {
        GL gl = Context.GL;

        gl.Uniform2(GetUniformLocation(name), value.X, value.Y);
    }

    public void SetUniform(string name, Vector3D<float> value)
    {
        GL gl = Context.GL;

        gl.Uniform3(GetUniformLocation(name), value.X, value.Y, value.Z);
    }

    public void SetUniform(string name, Vector4D<float> value)
    {
        GL gl = Context.GL;

        gl.Uniform4(GetUniformLocation(name), value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(string name, Matrix2X2<float> value)
    {
        GL gl = Context.GL;

        gl.UniformMatrix2(GetUniformLocation(name), 1, false, (float*)&value);
    }

    public void SetUniform(string name, Matrix3X3<float> value)
    {
        GL gl = Context.GL;

        gl.UniformMatrix3(GetUniformLocation(name), 1, false, (float*)&value);
    }

    public void SetUniform(string name, Matrix4X4<float> value)
    {
        GL gl = Context.GL;

        gl.UniformMatrix4(GetUniformLocation(name), 1, false, (float*)&value);
    }

    public void Bind()
    {
        GL gl = Context.GL;

        gl.UseProgram(Handle);

        if (IsDepthTest)
        {
            gl.Enable(EnableCap.DepthTest);
        }
        else
        {
            gl.Disable(EnableCap.DepthTest);
        }

        gl.DepthMask(IsDepthWrite);

        gl.DepthFunc((GLEnum)DepthFunction);

        if (IsStencilTest)
        {
            gl.Enable(EnableCap.StencilTest);
        }
        else
        {
            gl.Disable(EnableCap.StencilTest);
        }

        gl.StencilFunc((GLEnum)StencilFunction, StencilReference, StencilMask);

        gl.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

        gl.StencilMask((uint)(IsStencilWrite ? 0xFF : 0x00));

        if (IsCullFace)
        {
            gl.Enable(EnableCap.CullFace);
        }
        else
        {
            gl.Disable(EnableCap.CullFace);
        }

        gl.CullFace((GLEnum)CullFaceMode);

        if (IsBlend)
        {
            gl.Enable(EnableCap.Blend);
        }
        else
        {
            gl.Disable(EnableCap.Blend);
        }

        gl.BlendFunc((GLEnum)SourceFactor, (GLEnum)DestinationFactor);

        gl.BlendEquation((GLEnum)BlendEquation);

        gl.ColorMask(IsColorWrite, IsColorWrite, IsColorWrite, IsColorWrite);
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.UseProgram(0);
    }
}