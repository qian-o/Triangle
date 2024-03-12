using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials;

public class EdgeDetectionMat(TrContext context) : GlobalMat(context, "EdgeDetection")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Vector4D<float> EdgeColor;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public Vector4D<float> EdgeColor { get; set; } = new(0.9215686274509803f, 0.6352941176470588f, 0.0392156862745098f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/EdgeDetection/EdgeDetection.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/EdgeDetection/EdgeDetection.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Overlay);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(IList<TrMesh> meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters()
        {
            EdgeColor = EdgeColor
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        foreach (TrMesh mesh in meshes)
        {
            Bind(mesh);
            mesh.Draw();
        }

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        Vector4D<float> color = EdgeColor;
        ImGuiHelper.ColorEdit4("EdgeColor", ref color);
        EdgeColor = color;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
