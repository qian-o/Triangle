using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Common.Models;
using Example01.Contracts.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Graphics;

namespace Example01.Materials.Chapter7;

public class SingleTextureMat(TrContext context) : GlobalMat(context, "SingleTexture")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMaterial
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;

        [FieldOffset(16)]
        public Vector4D<float> Specular;

        [FieldOffset(32)]
        public float Gloss;

        [FieldOffset(48)]
        public Vector4D<float> MainTexST;
    }
    #endregion

    private TrBuffer<UniMaterial> uboMaterial = null!;

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public Vector4D<float> Specular { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public float Gloss { get; set; } = 20.0f;

    public TrTexture MainTex { get; } = new TrTexture(context);

    public Vector4D<float> MainTexST { get; set; } = new(1.0f, 1.0f, 0.0f, 0.0f);

    public override TrRenderPass CreateRenderPass()
    {
        MainTex.LoadImage("Resources/Textures/Chapter7/Brick_Diffuse.JPG".PathFormatter());

        uboMaterial = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter7/SingleTexture/SingleTexture.vert.spv".PathFormatter());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter7/SingleTexture/SingleTexture.frag.spv".PathFormatter());

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
            Color = Color,
            Specular = Specular,
            Gloss = Gloss,
            MainTexST = MainTexST
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboMaterial);
        renderPipeline.BindUniformBlock(0, MainTex);

        mesh.Draw();
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 color = Color.ToSystem();
        ImGui.ColorEdit4("Color", ref color);
        Color = color.ToGeneric();

        Vector4 specular = Specular.ToSystem();
        ImGui.ColorEdit4("Specular", ref specular);
        Specular = specular.ToGeneric();

        float gloss = Gloss;
        ImGui.DragFloat("Gloss", ref gloss, 0.1f, 8.0f, 256.0f);
        Gloss = gloss;

        Vector4 mainTexST = MainTexST.ToSystem();
        ImGui.DragFloat4("MainTexST", ref mainTexST, 0.1f, 0.0f, 100.0f);
        MainTexST = mainTexST.ToGeneric();
    }

    protected override void DestroyCore(bool disposing = false)
    {
        MainTex.Dispose();
        uboMaterial.Dispose();
    }
}
