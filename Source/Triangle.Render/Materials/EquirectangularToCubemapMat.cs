using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class EquirectangularToCubemapMat(TrContext context) : GlobalMat(context, "EquirectangularToCubemap")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Matrix4X4<float> View;

        [FieldOffset(64)]
        public Matrix4X4<float> Projection;

        [FieldOffset(128)]
        public bool GammaCorrection;

        [FieldOffset(132)]
        public float Gamma;

        [FieldOffset(136)]
        public float Exposure;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public Matrix4X4<float> View { get; set; }

    public Matrix4X4<float> Projection { get; set; }

    public bool GammaCorrection { get; set; }

    public float Gamma { get; set; } = 2.2f;

    public float Exposure { get; set; } = 1.0f;

    protected override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/EquirectangularToCubemap/EquirectangularToCubemap.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/EquirectangularToCubemap/EquirectangularToCubemap.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(IEnumerable<TrMesh> meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters
        {
            View = View,
            Projection = Projection,
            GammaCorrection = GammaCorrection,
            Gamma = Gamma,
            Exposure = Exposure
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
        bool gammaCorrection = GammaCorrection;
        ImGuiHelper.Checkbox("Gamma Correction", ref gammaCorrection);
        GammaCorrection = gammaCorrection;

        if (GammaCorrection)
        {
            float gamma = Gamma;
            ImGuiHelper.DragFloat("Gamma", ref gamma, 0.01f, 0.1f, 3.0f);
            Gamma = gamma;

            float exposure = Exposure;
            ImGuiHelper.DragFloat("Exposure", ref exposure, 0.01f, 0.1f, 10.0f);
            Exposure = exposure;
        }

        AdjustChannel("Sky Map", 0);
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
