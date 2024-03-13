using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Materials.Chapter7;

public class MaskTextureMat(TrContext context) : GlobalMat(context, "MaskTexture")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniMaterial
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;

        [FieldOffset(16)]
        public float BumpScale;

        [FieldOffset(20)]
        public float SpecularScale;

        [FieldOffset(32)]
        public Vector4D<float> Specular;

        [FieldOffset(48)]
        public float Gloss;
    }
    #endregion

    private TrBuffer<UniMaterial> uboMaterial = null!;

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public float BumpScale { get; set; } = 1.0f;

    public float SpecularScale { get; set; } = 1.0f;

    public Vector4D<float> Specular { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public float Gloss { get; set; } = 20.0f;

    public override TrRenderPass CreateRenderPass()
    {
        uboMaterial = new(Context);

        Channel0 = TrTextureManager.Texture("Resources/Textures/Chapter07/Road_Diffuse.tga".Path());
        Channel1 = TrTextureManager.Texture("Resources/Textures/Chapter07/Road_Normal.tga".Path());
        Channel2 = TrTextureManager.Texture("Resources/Textures/Chapter07/Road_Specular.tga".Path());

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Chapter7/MaskTexture/MaskTexture.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Chapter7/MaskTexture/MaskTexture.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore(IEnumerable<TrMesh> meshes, GlobalParameters globalParameters)
    {
        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboMaterial.SetData(new UniMaterial()
        {
            Color = Color,
            BumpScale = BumpScale,
            SpecularScale = SpecularScale,
            Specular = Specular,
            Gloss = Gloss
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboMaterial);

        foreach (TrMesh mesh in meshes)
        {
            
            mesh.Draw();
        }

        renderPipeline.Unbind();
    }

    protected override void ControllerCore()
    {
        Vector4D<float> color = Color;
        ImGuiHelper.ColorEdit4("Color", ref color);
        Color = color;

        AdjustChannel("Main Tex", 0);

        AdjustChannel("Normal Map", 1);

        float bumpScale = BumpScale;
        ImGuiHelper.DragFloat("Normal Scale", ref bumpScale, 0.01f);
        BumpScale = bumpScale;

        AdjustChannel("Specular Mask", 2);

        float specularScale = SpecularScale;
        ImGuiHelper.DragFloat("Specular Scale", ref specularScale, 0.01f);
        SpecularScale = specularScale;

        Vector4D<float> specular = Specular;
        ImGuiHelper.ColorEdit4("Specular", ref specular);
        Specular = specular;

        float gloss = Gloss;
        ImGuiHelper.SliderFloat("Gloss", ref gloss, 8.0f, 256.0f);
        Gloss = gloss;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboMaterial.Dispose();
    }
}
