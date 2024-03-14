using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Structs;
using AttribLocation = uint;

namespace Triangle.Core.Graphics;

public unsafe class TrMesh : TrGraphics<TrContext>
{
    public const AttribLocation InPosition = 0;
    public const AttribLocation InNormal = 1;
    public const AttribLocation InTangent = 2;
    public const AttribLocation InBitangent = 3;
    public const AttribLocation InColor = 4;
    public const AttribLocation InTexCoord = 5;

    private readonly TrVertex[] _vertices;
    private readonly uint[] _indices;

    public TrMesh(TrContext context, string name, TrVertex[] vertices, uint[] indices) : base(context)
    {
        _vertices = vertices;
        _indices = indices;

        GL gl = Context.GL;

        Name = name;
        Handle = gl.CreateVertexArray();
        VerticesBuffer = gl.CreateBuffer();
        IndicesBuffer = gl.CreateBuffer();

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

        VertexAttributePointer(InPosition, 3, nameof(TrVertex.Position));
        VertexAttributePointer(InNormal, 3, nameof(TrVertex.Normal));
        VertexAttributePointer(InTangent, 3, nameof(TrVertex.Tangent));
        VertexAttributePointer(InBitangent, 3, nameof(TrVertex.Bitangent));
        VertexAttributePointer(InColor, 4, nameof(TrVertex.Color));
        VertexAttributePointer(InTexCoord, 2, nameof(TrVertex.TexCoord));
    }

    public string Name { get; }

    public uint VerticesBuffer { get; }

    public uint IndicesBuffer { get; }

    public int VerticesLength => _vertices.Length;

    public int IndicesLength => _indices.Length;

    public ReadOnlyCollection<TrVertex> Vertices => _vertices.AsReadOnly();

    public ReadOnlyCollection<uint> Indices => _indices.AsReadOnly();

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteVertexArray(Handle);
        gl.DeleteBuffer(VerticesBuffer);
        gl.DeleteBuffer(IndicesBuffer);
    }

    private void VertexAttributePointer(uint index, int size, string fieldName)
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

    public void DrawInstanced(int count)
    {
        GL gl = Context.GL;

        gl.BindVertexArray(Handle);
        gl.DrawElementsInstanced(GLEnum.Triangles, (uint)IndicesLength, GLEnum.UnsignedInt, null, (uint)count);
        gl.BindVertexArray(0);
    }
}
