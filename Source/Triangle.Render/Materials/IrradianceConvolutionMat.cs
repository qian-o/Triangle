using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class IrradianceConvolutionMat(TrContext context) : GlobalMat(context, "IrradianceConvolution")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Matrix4X4<float> View;

        [FieldOffset(64)]
        public Matrix4X4<float> Projection;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public Matrix4X4<float> View { get; set; }

    public Matrix4X4<float> Projection { get; set; }

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/IrradianceConvolution/IrradianceConvolution.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/IrradianceConvolution/IrradianceConvolution.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh[] meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters
        {
            View = View,
            Projection = Projection
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        foreach (TrMesh mesh in meshes)
        {

            mesh.Draw();
        }

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        AdjustChannel("Sky Map", 0);
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
