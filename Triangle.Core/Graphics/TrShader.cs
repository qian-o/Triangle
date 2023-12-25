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

        Init();
    }

    public TrShaderType ShaderType { get; }

    public string Source { get; }

    protected override void Init()
    {
        GL gl = Context.GL;

        GLEnum shaderType = ShaderType switch
        {
            Enums.TrShaderType.Vertex => GLEnum.VertexShader,
            Enums.TrShaderType.Geometry => GLEnum.GeometryShader,
            Enums.TrShaderType.Fragment => GLEnum.FragmentShader,
            Enums.TrShaderType.Compute => GLEnum.ComputeShader,
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

    protected override void Destroy()
    {
        GL gl = Context.GL;

        gl.DeleteShader(Handle);
    }
}
