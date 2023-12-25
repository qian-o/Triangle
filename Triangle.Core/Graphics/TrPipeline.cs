using Silk.NET.OpenGLES;
using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Graphics;

public class TrPipeline : TrGraphics<TrContext>
{
    internal TrPipeline(TrContext context, TrShader[] shaders) : base(context)
    {
        Shaders = new ReadOnlyCollection<TrShader>(shaders);

        Init();
    }

    public bool IsDepthTest { get; set; } = true;

    public bool IsDepthWrite { get; set; } = true;

    public bool IsStencilTest { get; set; } = true;

    public bool IsBlend { get; set; } = true;

    public ReadOnlyCollection<TrShader> Shaders { get; }

    protected override void Init()
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

    protected override void Destroy()
    {
        GL gl = Context.GL;

        foreach (TrShader shader in Shaders)
        {
            gl.DetachShader(Handle, shader.Handle);
        }

        gl.DeleteProgram(Handle);
    }
}
