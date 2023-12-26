using Silk.NET.OpenGLES;
using Triangle.Core.Contracts;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core;

public class TrContext(GL gl) : TrObject
{
    private List<TrShader> _shaders = [];
    private List<TrRenderPipeline> _renderPipelines = [];
    private List<TrRenderPass> _renderPasses = [];

    public GL GL { get; } = gl;

    public TrShader CreateShader(TrShaderType shaderType, string source)
    {
        TrShader shader = new(this, shaderType, source);
        _shaders.Add(shader);

        return shader;
    }

    public TrRenderPipeline CreateRenderPipeline(TrDescriptor descriptor, IList<TrShader> shaders)
    {
        TrRenderPipeline renderPipeline = new(this, descriptor, shaders);
        _renderPipelines.Add(renderPipeline);

        return renderPipeline;
    }

    public TrRenderPass CreateRenderPass(TrRenderLayer renderLayer, IList<TrRenderPipeline> pipelines)
    {
        TrRenderPass renderPass = new(this, renderLayer, pipelines);
        _renderPasses.Add(renderPass);

        return renderPass;
    }

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            foreach (TrRenderPass renderPass in _renderPasses)
            {
                renderPass.Dispose();
            }

            foreach (TrRenderPipeline pipeline in _renderPipelines)
            {
                pipeline.Dispose();
            }

            foreach (TrShader shader in _shaders)
            {
                shader.Dispose();
            }
        }

        _renderPasses = [];
        _renderPipelines = [];
        _shaders = [];
    }
}