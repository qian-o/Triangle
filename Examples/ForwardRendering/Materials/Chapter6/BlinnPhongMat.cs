using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Common.Models;
using ForwardRendering.Contracts.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;

namespace ForwardRendering.Materials.Chapter6;

public class BlinnPhongMat(TrContext context) : GlobalMat(context, "BlinnPhong")
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
        uboMaterial = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter6/BlinnPhong.vert.spv");
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter6/BlinnPhong.frag.spv");

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore([NotNull] TrMesh mesh, [NotNull] TrSceneParameters sceneParameters)
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

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 diffuse = Diffuse.ToSystem();
        ImGui.ColorEdit4("Diffuse", ref diffuse);
        Diffuse = diffuse.ToGeneric();

        Vector4 specular = Specular.ToSystem();
        ImGui.ColorEdit4("Specular", ref specular);
        Specular = specular.ToGeneric();

        float gloss = Gloss;
        ImGui.DragFloat("Gloss", ref gloss, 0.1f, 8.0f, 256f);
        Gloss = gloss;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboMaterial.Dispose();
    }
}
