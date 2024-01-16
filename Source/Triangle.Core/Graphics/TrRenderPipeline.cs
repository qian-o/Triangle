using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

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
                IsCullFace = true;
                TriangleFace = TrTriangleFace.Back;
                FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
                IsBlend = true;
                SourceFactor = TrBlendFactor.SrcAlpha;
                DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                BlendEquation = TrBlendEquation.Add;
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
                IsMultisample = true;
                break;
            case TrRenderLayer.Overlay:
                IsColorWrite = true;
                IsDepthTest = false;
                IsDepthWrite = false;
                DepthFunction = TrDepthFunction.Less;
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
                IsMultisample = true;
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

    public void SetUniform<T>(string name, T value) where T : struct
    {
        foreach (FieldInfo field in value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            Type fieldType = field.FieldType;

            if (fieldType == typeof(int))
            {
                SetUniform($"{name}.{field.Name}", (int)field.GetValue(value)!);
            }
            else if (fieldType == typeof(float))
            {
                SetUniform($"{name}.{field.Name}", (float)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Vector2D<float>))
            {
                SetUniform($"{name}.{field.Name}", (Vector2D<float>)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Vector3D<float>))
            {
                SetUniform($"{name}.{field.Name}", (Vector3D<float>)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Vector4D<float>))
            {
                SetUniform($"{name}.{field.Name}", (Vector4D<float>)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Matrix2X2<float>))
            {
                SetUniform($"{name}.{field.Name}", (Matrix2X2<float>)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Matrix3X3<float>))
            {
                SetUniform($"{name}.{field.Name}", (Matrix3X3<float>)field.GetValue(value)!);
            }
            else if (fieldType == typeof(Matrix4X4<float>))
            {
                SetUniform($"{name}.{field.Name}", (Matrix4X4<float>)field.GetValue(value)!);
            }
            else
            {
                throw new NotSupportedException($"不支持的类型：{fieldType}。");
            }
        }
    }

    public void BindUniformBlock<T>(uint bindingPoint, TrBuffer<T> ubo) where T : unmanaged
    {
        GL gl = Context.GL;

        gl.BindBufferBase(GLEnum.UniformBuffer, bindingPoint, ubo.Handle);
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

        if (IsMultisample)
        {
            gl.Enable(GLEnum.Multisample);
        }
        else
        {
            gl.Disable(GLEnum.Multisample);
        }
    }

    public void Unbind()
    {
        GL gl = Context.GL;

        gl.UseProgram(0);
    }
}