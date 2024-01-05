using Common.Models;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials;

public unsafe class DiffuseVertexLevelMat(TrContext context) : TrMaterial<TrParameter>(context)
{
    private TrRenderPipeline renderPipeline = null!;

    public Vector4D<float> Diffuse { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override TrRenderPass CreateRenderPass()
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/DiffuseVertexLevelShader.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/DiffuseVertexLevelShader.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    public override void Draw([NotNull] TrMesh mesh, [NotNull] TrParameter parameter)
    {
        GL gl = Context.GL;

        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            renderPipeline.SetUniform("Uni_Model", parameter.Model);
            renderPipeline.SetUniform("Uni_View", parameter.Camera.View);
            renderPipeline.SetUniform("Uni_Projection", parameter.Camera.Projection);

            gl.BindVertexArray(mesh.Handle);
            gl.DrawElements(GLEnum.Triangles, (uint)mesh.IndexLength, GLEnum.UnsignedInt, null);
            gl.BindVertexArray(0);

            renderPipeline.Unbind();
        }
    }

    public override void ImGuiEdit()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
