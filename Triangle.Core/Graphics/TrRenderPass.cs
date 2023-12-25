using System.Collections.ObjectModel;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public class TrRenderPass : TrGraphics<TrContext>
{
    internal TrRenderPass(TrContext context, TrRenderLayer renderLayer, TrPipeline[] pipelines) : base(context)
    {
        RenderLayer = renderLayer;
        Pipelines = new ReadOnlyCollection<TrPipeline>(pipelines);

        Initialize();
    }

    public TrRenderLayer RenderLayer { get; set; }

    public ReadOnlyCollection<TrPipeline> Pipelines { get; }

    protected override void Initialize()
    {
        bool isDepthTest;
        bool isDepthWrite;
        TrDepthFunction depthFunction;

        bool isStencilTest;
        bool isStencilWrite;
        TrStencilFunction stencilFunction;
        int stencilReference;
        int stencilMask;

        bool isCullFace;
        TrCullFaceMode cullFaceMode;

        bool isBlend;
        TrBlendFactor sourceFactor;
        TrBlendFactor destinationFactor;

        switch (RenderLayer)
        {
            case TrRenderLayer.Background:
                isDepthTest = true;
                isDepthWrite = true;
                depthFunction = TrDepthFunction.LessOrEqual;
                isStencilTest = false;
                isStencilWrite = false;
                stencilFunction = TrStencilFunction.Always;
                stencilReference = 0;
                stencilMask = 0xFF;
                isCullFace = false;
                cullFaceMode = TrCullFaceMode.CounterClockwise;
                isBlend = false;
                sourceFactor = TrBlendFactor.One;
                destinationFactor = TrBlendFactor.Zero;
                break;
            case TrRenderLayer.Geometry:
                isDepthTest = true;
                isDepthWrite = true;
                depthFunction = TrDepthFunction.Less;
                isStencilTest = true;
                isStencilWrite = true;
                stencilFunction = TrStencilFunction.Always;
                stencilReference = 0;
                stencilMask = 0xFF;
                isCullFace = true;
                cullFaceMode = TrCullFaceMode.CounterClockwise;
                isBlend = false;
                sourceFactor = TrBlendFactor.One;
                destinationFactor = TrBlendFactor.Zero;
                break;
            case TrRenderLayer.Opaque:
                isDepthTest = true;
                isDepthWrite = true;
                depthFunction = TrDepthFunction.Less;
                isStencilTest = true;
                isStencilWrite = true;
                stencilFunction = TrStencilFunction.Always;
                stencilReference = 0;
                stencilMask = 0xFF;
                isCullFace = true;
                cullFaceMode = TrCullFaceMode.CounterClockwise;
                isBlend = false;
                sourceFactor = TrBlendFactor.One;
                destinationFactor = TrBlendFactor.Zero;
                break;
            case TrRenderLayer.Transparent:
                isDepthTest = true;
                isDepthWrite = false;
                depthFunction = TrDepthFunction.Less;
                isStencilTest = true;
                isStencilWrite = true;
                stencilFunction = TrStencilFunction.Always;
                stencilReference = 0;
                stencilMask = 0xFF;
                isCullFace = true;
                cullFaceMode = TrCullFaceMode.CounterClockwise;
                isBlend = true;
                sourceFactor = TrBlendFactor.SrcAlpha;
                destinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                break;
            case TrRenderLayer.Overlay:
                isDepthTest = false;
                isDepthWrite = false;
                depthFunction = TrDepthFunction.Less;
                isStencilTest = false;
                isStencilWrite = false;
                stencilFunction = TrStencilFunction.Always;
                stencilReference = 0;
                stencilMask = 0xFF;
                isCullFace = false;
                cullFaceMode = TrCullFaceMode.CounterClockwise;
                isBlend = true;
                sourceFactor = TrBlendFactor.SrcAlpha;
                destinationFactor = TrBlendFactor.OneMinusSrcAlpha;
                break;
            default:
                throw new NotSupportedException();
        }

        foreach (TrPipeline pipeline in Pipelines)
        {
            pipeline.IsDepthTest = isDepthTest;
            pipeline.IsDepthWrite = isDepthWrite;
            pipeline.DepthFunction = depthFunction;

            pipeline.IsStencilTest = isStencilTest;
            pipeline.IsStencilWrite = isStencilWrite;
            pipeline.StencilFunction = stencilFunction;
            pipeline.StencilReference = stencilReference;
            pipeline.StencilMask = stencilMask;

            pipeline.IsCullFace = isCullFace;
            pipeline.CullFaceMode = cullFaceMode;

            pipeline.IsBlend = isBlend;
            pipeline.SourceFactor = sourceFactor;
            pipeline.DestinationFactor = destinationFactor;
        }
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
