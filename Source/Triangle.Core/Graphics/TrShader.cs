using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrShader : TrGraphics<TrContext>
{
    public TrShader(TrContext context, TrShaderType shaderType, string source) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateShader(shaderType.ToGL());

        gl.ShaderSource(Handle, source);
        gl.CompileShader(Handle);

        string error = gl.GetShaderInfoLog(Handle);

        if (!string.IsNullOrEmpty(error))
        {
            gl.DeleteShader(Handle);

            throw new TrException($"{shaderType}: {error}");
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteShader(Handle);
    }
}
