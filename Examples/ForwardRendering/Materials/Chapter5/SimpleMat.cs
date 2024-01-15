using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Common.Models;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials.Chapter5;

public class SimpleMat(TrContext context) : TrMaterial<TrParameter>(context, "Simple")
{
    private TrRenderPipeline renderPipeline = null!;

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/Chapter5/Simple.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/Chapter5/Simple.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));

            renderPipeline.SetUniform("Uni_Model", parameter.Model);
            renderPipeline.SetUniform("Uni_View", parameter.Camera.View);
            renderPipeline.SetUniform("Uni_Projection", parameter.Camera.Projection);
            renderPipeline.SetUniform("Uni_Color", Color);

            mesh.Draw();

            renderPipeline.Unbind();
        }
    }

    protected override void AdjustImGuiPropertiesCore()
    {
        Vector4 color = Color.ToSystem();
        ImGui.ColorEdit4("Color", ref color);
        Color = color.ToGeneric();
    }

    protected override void Destroy(bool disposing = false)
    {
        RenderPass.Dispose();

        renderPipeline.Dispose();
    }
}
