using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core.Graphics;

public unsafe class TrMesh : TrGraphics<TrContext>
{
    public TrMesh(TrContext context, TrVertex[] vertices, uint[] indices) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateVertexArray();
        VerticesBuffer = gl.CreateBuffer();
        IndicesBuffer = gl.CreateBuffer();
        VerticesLength = vertices.Length;
        IndicesLength = indices.Length;

        fixed (TrVertex* verticesPtr = vertices)
        {
            gl.NamedBufferStorage(VerticesBuffer, (uint)(vertices.Length * sizeof(TrVertex)), verticesPtr, (uint)GLEnum.None);
        }

        fixed (uint* indicesPtr = indices)
        {
            gl.NamedBufferStorage(IndicesBuffer, (uint)(indices.Length * sizeof(uint)), indicesPtr, (uint)GLEnum.None);
        }

        gl.VertexArrayVertexBuffer(Handle, 0, VerticesBuffer, 0, (uint)sizeof(TrVertex));
        gl.VertexArrayElementBuffer(Handle, IndicesBuffer);
    }

    public uint VerticesBuffer { get; }

    public uint IndicesBuffer { get; }

    public int VerticesLength { get; }

    public int IndicesLength { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteVertexArray(Handle);
        gl.DeleteBuffer(VerticesBuffer);
        gl.DeleteBuffer(IndicesBuffer);
    }

    public void VertexAttributePointer(uint index, int size, string fieldName)
    {
        GL gl = Context.GL;

        gl.EnableVertexArrayAttrib(Handle, index);
        gl.VertexArrayAttribFormat(Handle, index, size, GLEnum.Float, false, (uint)Marshal.OffsetOf<TrVertex>(fieldName));
        gl.VertexArrayAttribBinding(Handle, index, 0);
    }

    public void Draw()
    {
        GL gl = Context.GL;

        gl.BindVertexArray(Handle);
        gl.DrawElements(GLEnum.Triangles, (uint)IndicesLength, GLEnum.UnsignedInt, null);
        gl.BindVertexArray(0);
    }
}
