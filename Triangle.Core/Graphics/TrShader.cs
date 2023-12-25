using Silk.NET.OpenGLES;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Graphics;

public class TrShader : TrGraphics<TrContext>
{
    internal TrShader(TrContext context, TrShaderType shaderType, string source) : base(context)
    {
        ShaderType = shaderType;
        Source = source;

        Initialize();
    }

    public TrShaderType ShaderType { get; }

    public string Source { get; }

    protected override void Initialize()
    {
        GL gl = Context.GL;

        GLEnum shaderType = ShaderType switch
        {
            TrShaderType.Vertex => GLEnum.VertexShader,
            TrShaderType.Geometry => GLEnum.GeometryShader,
            TrShaderType.Fragment => GLEnum.FragmentShader,
            TrShaderType.Compute => GLEnum.ComputeShader,
            _ => throw new NotSupportedException()
        };

        Handle = gl.CreateShader(shaderType);

        gl.ShaderSource(Handle, Source);
        gl.CompileShader(Handle);

        string error = gl.GetShaderInfoLog(Handle);

        if (!string.IsNullOrEmpty(error))
        {
            Destroy();

            throw new TrException($"{shaderType}: {error}");
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteShader(Handle);
    }
}
