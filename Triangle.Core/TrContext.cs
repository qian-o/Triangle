using Silk.NET.OpenGLES;
using Triangle.Core.Contracts;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;

namespace Triangle.Core;

public class TrContext(GL gl) : TrObject
{
    private List<TrPipeline> _pipelines = [];
    private List<TrShader> _shaders = [];

    public GL GL { get; } = gl;

    public TrPipeline CreatePipeline(TrShader[] shaders)
    {
        TrPipeline pipeline = new(this, shaders);
        _pipelines.Add(pipeline);

        return pipeline;
    }

    public TrShader CreateShader(TrShaderType shaderType, string source)
    {
        TrShader shader = new(this, shaderType, source);
        _shaders.Add(shader);

        return shader;
    }

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            foreach (TrPipeline pipeline in _pipelines)
            {
                pipeline.Dispose();
            }

            foreach (TrShader shader in _shaders)
            {
                shader.Dispose();
            }
        }

        _pipelines = [];
        _shaders = [];
    }
}
