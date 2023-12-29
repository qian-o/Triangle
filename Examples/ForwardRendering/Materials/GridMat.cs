using Common;
using ImGuiNET;
using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials;

public unsafe class GridMat(TrContext context) : TrMaterial(context)
{
    private TrRenderPipeline renderPipeline = null!;

    public float Distance { get; set; } = 6.0f;

    public float PrimaryScale { get; set; }

    public float SecondaryScale { get; set; }

    public float GridIntensity { get; set; } = 1.0f;

    public float Fade { get; set; }

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/GridShader.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/GridShader.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Geometry);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, params object[] args)
    {
        if (args == null || args.Length != 1)
        {
            return;
        }

        Camera camera = (Camera)args[0];

        GL gl = Context.GL;

        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));

            renderPipeline.SetUniform("Uni_View", camera.View);
            renderPipeline.SetUniform("Uni_Projection", camera.Projection);
            renderPipeline.SetUniform("Uni_Near", camera.Near);
            renderPipeline.SetUniform("Uni_Far", camera.Far);
            renderPipeline.SetUniform("Uni_PrimaryScale", PrimaryScale);
            renderPipeline.SetUniform("Uni_SecondaryScale", SecondaryScale);
            renderPipeline.SetUniform("Uni_GridIntensity", GridIntensity);
            renderPipeline.SetUniform("Uni_Fade", Fade);

            gl.BindVertexArray(mesh.Handle);
            gl.DrawElements(GLEnum.Triangles, (uint)mesh.IndexLength, GLEnum.UnsignedInt, null);
            gl.BindVertexArray(0);

            renderPipeline.Unbind();
        }
    }

    public override void ImGuiEdit()
    {
        float distance = Distance;
        ImGui.SliderFloat("Distance", ref distance, 0.0f, 10.0f);
        Distance = distance;

        float gridIntensity = GridIntensity;
        ImGui.SliderFloat("Grid Intensity", ref gridIntensity, 0.0f, 1.0f);
        GridIntensity = gridIntensity;

        double logDistance = Math.Log2(Distance);
        double upperDistance = Math.Pow(2.0, Math.Floor(logDistance) + 1);
        double lowerDistance = Math.Pow(2.0, Math.Floor(logDistance));
        Fade = Convert.ToSingle((Distance - lowerDistance) / (upperDistance - lowerDistance));

        double level = -Math.Floor(logDistance);
        PrimaryScale = Convert.ToSingle(Math.Pow(2.0, level));
        SecondaryScale = Convert.ToSingle(Math.Pow(2.0, level + 1));
    }

    protected override void Destroy(bool disposing = false)
    {
        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
