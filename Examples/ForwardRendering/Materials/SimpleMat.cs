using Common;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Structs;

namespace ForwardRendering.Materials;

public unsafe class SimpleMat : TrMaterial
{
    private readonly TrRenderPipeline renderPipeline;

    public SimpleMat(TrContext context) : base(context)
    {
        using TrShader vert = new(Context, TrShaderType.Vertex, File.ReadAllText("Resources/Shaders/SimpleShader.vert"));
        using TrShader frag = new(Context, TrShaderType.Fragment, File.ReadAllText("Resources/Shaders/SimpleShader.frag"));

        renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        RenderPass = new(Context, [renderPipeline]);
    }

    public Vector4D<float> Color { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public override void Draw([NotNull] TrMesh mesh, params object[] args)
    {
        if (args == null || args.Length != 2)
        {
            return;
        }

        Camera camera = (Camera)args[0];
        Matrix4X4<float> model = (Matrix4X4<float>)args[1];

        GL gl = Context.GL;

        foreach (TrRenderPipeline renderPipeline in RenderPass!.RenderPipelines)
        {
            renderPipeline.Bind();

            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Position"), 3, nameof(TrVertex.Position));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_Normal"), 3, nameof(TrVertex.Normal));
            mesh.VertexAttributePointer((uint)renderPipeline.GetAttribLocation("In_TexCoord"), 2, nameof(TrVertex.TexCoord));

            renderPipeline.SetUniform("Uni_Model", model);
            renderPipeline.SetUniform("Uni_View", camera.View);
            renderPipeline.SetUniform("Uni_Projection", camera.Projection);
            renderPipeline.SetUniform("Uni_Color", Color);

            gl.BindVertexArray(mesh.Handle);
            gl.DrawElements(GLEnum.Triangles, (uint)mesh.IndexLength, GLEnum.UnsignedInt, null);
            gl.BindVertexArray(0);

            renderPipeline.Unbind();
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        renderPipeline.Dispose();

        RenderPass?.Dispose();
    }
}
