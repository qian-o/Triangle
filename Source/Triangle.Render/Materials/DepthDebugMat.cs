using System.Runtime.InteropServices;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class DepthDebugMat(TrContext context) : GlobalMat(context, "DepthDebug")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public float NearPlane;

        [FieldOffset(4)]
        public float FarPlane;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public float NearPlane { get; set; }

    public float FarPlane { get; set; }

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/DepthDebug/DepthDebug.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/DepthDebug/DepthDebug.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void AssemblePipeline(TrRenderPipeline renderPipeline, GlobalParameters globalParameters)
    {
        uboParameters.SetData(new UniParameters()
        {
            NearPlane = NearPlane,
            FarPlane = FarPlane
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);
    }

    protected override void RenderPipeline(TrRenderPipeline renderPipeline, TrMesh[] meshes, GlobalParameters globalParameters)
    {
        foreach (TrMesh mesh in meshes)
        {
            mesh.Draw();
        }
    }

    protected override void ControllerCore()
    {
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
