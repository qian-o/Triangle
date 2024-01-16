using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Render.Structs;

namespace Triangle.Render.Graphics;

public unsafe class TrMesh : TrGraphics<TrContext>
{
    public TrMesh(TrContext context, [NotNull] TrVertex[] vertices, [NotNull] uint[] indices) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.GenVertexArray();
        ArrayBuffer = gl.GenBuffer();
        IndexBuffer = gl.GenBuffer();
        IndexLength = indices.Length;

        gl.BindVertexArray(Handle);

        gl.BindBuffer(GLEnum.ArrayBuffer, ArrayBuffer);
        gl.BufferData<TrVertex>(GLEnum.ArrayBuffer, (uint)(vertices.Length * sizeof(TrVertex)), vertices, GLEnum.StaticDraw);

        gl.BindBuffer(GLEnum.ElementArrayBuffer, IndexBuffer);
        gl.BufferData<uint>(GLEnum.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), indices, GLEnum.StaticDraw);

        gl.BindVertexArray(0);
    }

    public uint ArrayBuffer { get; }

    public uint IndexBuffer { get; }

    public int IndexLength { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteVertexArray(Handle);
        gl.DeleteBuffer(ArrayBuffer);
        gl.DeleteBuffer(IndexBuffer);
    }

    public void VertexAttributePointer(uint index, int size, string fieldName)
    {
        GL gl = Context.GL;

        gl.BindVertexArray(Handle);

        gl.BindBuffer(GLEnum.ArrayBuffer, ArrayBuffer);
        gl.VertexAttribPointer(index, size, GLEnum.Float, false, (uint)sizeof(TrVertex), (void*)Marshal.OffsetOf<TrVertex>(fieldName));
        gl.EnableVertexAttribArray(index);
        gl.BindBuffer(GLEnum.ArrayBuffer, 0);

        gl.BindVertexArray(0);
    }

    public void Draw()
    {
        GL gl = Context.GL;

        gl.BindVertexArray(Handle);
        gl.DrawElements(GLEnum.Triangles, (uint)IndexLength, GLEnum.UnsignedInt, null);
        gl.BindVertexArray(0);
    }
}
