using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Render.Structs;

namespace Triangle.Render.Graphics;

public unsafe class TrMesh : TrGraphics<TrContext>
{
    public TrMesh(TrContext context, [NotNull] TrVertex[] vertices) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.GenBuffer();

        gl.BindBuffer(GLEnum.ArrayBuffer, Handle);
        gl.BufferData<TrVertex>(GLEnum.ArrayBuffer, (uint)(vertices.Length * sizeof(TrVertex)), vertices, BufferUsageARB.StaticDraw);
        gl.BindBuffer(GLEnum.ArrayBuffer, 0);
    }

    protected override void Destroy(bool disposing = false)
    {

    }
}
