using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ImGuiNET;
using Triangle.App.Contracts.Materials;
using Triangle.App.Models;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;

namespace Triangle.App.Materials;

public class GridMat(TrContext context) : GlobalMat(context, "Grid")
{
    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public float Near;

        [FieldOffset(4)]
        public float Far;

        [FieldOffset(8)]
        public float PrimaryScale;

        [FieldOffset(12)]
        public float SecondaryScale;

        [FieldOffset(16)]
        public float GridIntensity;

        [FieldOffset(20)]
        public float Fade;
    }
    #endregion

    private TrBuffer<UniParameters> uboParameters = null!;

    public float Distance { get; set; } = 6.0f;

    public float GridIntensity { get; set; } = 1.0f;

    public override TrRenderPass CreateRenderPass()
    {
        uboParameters = new(Context, TrBufferTarget.UniformBuffer, TrBufferUsage.Dynamic);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/Grid/Grid.vert.spv".PathFormatter());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/Grid/Grid.frag.spv".PathFormatter());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    protected override void DrawCore([NotNull] TrMesh mesh, [NotNull] TrSceneParameters sceneParameters)
    {
        double logDistance = Math.Log2(Distance);
        double upperDistance = Math.Pow(2.0, Math.Floor(logDistance) + 1);
        double lowerDistance = Math.Pow(2.0, Math.Floor(logDistance));
        float fade = Convert.ToSingle((Distance - lowerDistance) / (upperDistance - lowerDistance));

        double level = -Math.Floor(logDistance);
        float primaryScale = Convert.ToSingle(Math.Pow(2.0, level));
        float secondaryScale = Convert.ToSingle(Math.Pow(2.0, level + 1));

        TrRenderPipeline renderPipeline = RenderPass.RenderPipelines[0];

        renderPipeline.Bind();

        uboParameters.SetData(new UniParameters()
        {
            Near = sceneParameters.Camera.Near,
            Far = sceneParameters.Camera.Far,
            PrimaryScale = primaryScale,
            SecondaryScale = secondaryScale,
            GridIntensity = GridIntensity,
            Fade = fade
        });

        renderPipeline.BindUniformBlock(UniformBufferBindingStart + 0, uboParameters);

        mesh.Draw();

        renderPipeline.Unbind();
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        float distance = Distance;
        ImGui.SliderFloat("Distance", ref distance, 0.0f, 10.0f);
        Distance = distance;

        float gridIntensity = GridIntensity;
        ImGui.SliderFloat("Grid Intensity", ref gridIntensity, 0.0f, 1.0f);
        GridIntensity = gridIntensity;
    }

    protected override void DestroyCore(bool disposing = false)
    {
        uboParameters.Dispose();
    }
}
