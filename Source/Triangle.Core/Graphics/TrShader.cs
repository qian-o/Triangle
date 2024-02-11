using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrShader : TrGraphics<TrContext>
{
    public TrShader(TrContext context, TrShaderType shaderType, string source, bool isSpv = true) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateShader(shaderType.ToGL());

        if (isSpv)
        {
            byte[] bytes = File.ReadAllBytes(source);
            uint size = (uint)bytes.Length;

            gl.ShaderBinary(1, Handle, GLEnum.ShaderBinaryFormatSpirV, bytes[0], size);
            gl.SpecializeShader(Handle, "main", 0, null, null);
        }
        else
        {
            gl.ShaderSource(Handle, source);
            gl.CompileShader(Handle);
        }

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
