using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Chapter6;

public class SpecularVertexLevelMat(TrContext context) : GlobalMat(context, "SpecularVertexLevel")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMaterial
    {
        [FieldOffset(0)]
        public Vector4D<float> Diffuse;

        [FieldOffset(16)]
        public Vector4D<float> Specular;

        [FieldOffset(32)]
        public float Gloss;
    }
    #endregion

    private TrBuffer<UniMaterial> uboMaterial = null!;

    public Vector4D<float> Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public Vector4D<float> Specular { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public float Gloss { get; set; } = 20.0f;

    public override TrRenderPass CreateRenderPass()
    {
        uboMaterial = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter6/SpecularVertexLevel/SpecularVertexLevel.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter6/SpecularVertexLevel/SpecularVertexLevel.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(TrMesh mesh, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboMaterial.SetData(new UniMaterial()
        {
            Diffuse = Diffuse,
            Specular = Specular,
            Gloss = Gloss
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboMaterial);

        mesh.Draw();

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        Vector4D<float> diffuse = Diffuse;
        ImGuiHelper.ColorEdit4("Diffuse", ref diffuse);
        Diffuse = diffuse;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboMaterial.Dispose();
    }
}
