using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Graphics;

public unsafe class TrPipeline : TrGraphics<TrContext>
{
    internal TrPipeline(TrContext context, TrShader[] shaders) : base(context)
    {
        Shaders = new ReadOnlyCollection<TrShader>(shaders);

        Initialize();
    }

    public bool IsDepthTest { get; set; } = true;

    public bool IsDepthWrite { get; set; } = true;

    public TrDepthFunction DepthFunction { get; set; } = TrDepthFunction.Less;

    public bool IsStencilTest { get; set; } = true;

    public int StencilMask { get; set; } = 0xFF;

    public TrStencilFunction StencilFunction { get; set; } = TrStencilFunction.Always;

    public int StencilReference { get; set; }

    public bool IsStencilWrite { get; set; } = true;

    public bool IsCullFace { get; set; } = true;

    public TrCullFaceMode CullFaceMode { get; set; } = TrCullFaceMode.CounterClockwise;

    public bool IsBlend { get; set; } = true;

    public TrBlendFactor SourceFactor { get; set; } = TrBlendFactor.SrcAlpha;

    public TrBlendFactor DestinationFactor { get; set; } = TrBlendFactor.OneMinusSrcAlpha;

    public ReadOnlyCollection<TrShader> Shaders { get; }

    public void SetUniform(string name, int value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.Uniform1(location, value);
    }

    public void SetUniform(string name, float value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.Uniform1(location, value);
    }

    public void SetUniform(string name, Vector2D<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.Uniform2(location, value.X, value.Y);
    }

    public void SetUniform(string name, Vector3D<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void SetUniform(string name, Vector4D<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }

    public void SetUniform(string name, Matrix2X2<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.UniformMatrix2(location, 1, false, (float*)&value);
    }

    public void SetUniform(string name, Matrix3X3<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.UniformMatrix3(location, 1, false, (float*)&value);
    }

    public void SetUniform(string name, Matrix4X4<float> value)
    {
        GL gl = Context.GL;

        int location = gl.GetUniformLocation(Handle, name);

        gl.UniformMatrix4(location, 1, false, (float*)&value);
    }

    protected override void Initialize()
    {
        GL gl = Context.GL;

        Handle = gl.CreateProgram();

        foreach (TrShader shader in Shaders)
        {
            gl.AttachShader(Handle, shader.Handle);
        }

        gl.LinkProgram(Handle);

        string error = gl.GetProgramInfoLog(Handle);

        if (!string.IsNullOrEmpty(error))
        {
            Destroy();

            throw new TrException(error);
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        foreach (TrShader shader in Shaders)
        {
            gl.DetachShader(Handle, shader.Handle);
        }

        gl.DeleteProgram(Handle);
    }
}
