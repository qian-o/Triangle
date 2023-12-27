using Silk.NET.OpenGLES;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Graphics;

public class TrShader : TrGraphics<TrContext>
{
    public TrShader(TrContext context, TrShaderType shaderType, string source) : base(context)
    {
        GL gl = Context.GL;

        GLEnum @enum = shaderType switch
        {
            TrShaderType.Vertex => GLEnum.VertexShader,
            TrShaderType.Geometry => GLEnum.GeometryShader,
            TrShaderType.Fragment => GLEnum.FragmentShader,
            TrShaderType.Compute => GLEnum.ComputeShader,
            _ => throw new NotSupportedException("不支持的着色器类型。")
        };

        Handle = gl.CreateShader(@enum);

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
