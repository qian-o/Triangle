using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;

namespace Triangle.Core.Graphics;

public unsafe class TrBuffer : TrGraphics<TrContext>
{
    public TrBuffer(TrContext context, TrBufferUsage bufferUsage, uint size) : base(context)
    {
        Size = size;
        BufferUsage = bufferUsage;

        {
            GL gl = Context.GL;

            Handle = gl.GenBuffer();

            GLEnum @enum = BufferUsage switch
            {
                TrBufferUsage.Static => GLEnum.StaticDraw,
                TrBufferUsage.Dynamic => GLEnum.DynamicDraw,
                TrBufferUsage.Stream => GLEnum.StreamDraw,
                _ => GLEnum.DynamicDraw,
            };

            gl.BindBuffer(GLEnum.ArrayBuffer, Handle);
            gl.BufferData(GLEnum.ArrayBuffer, Size, null, @enum);
            gl.BindBuffer(GLEnum.ArrayBuffer, 0);
        }
    }

    public uint Size { get; }

    public TrBufferUsage BufferUsage { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);
    }

    public void SetData<T>([NotNull] T[] data, uint offset = 0) where T : unmanaged
    {
        GL gl = Context.GL;

        gl.BindBuffer(GLEnum.ArrayBuffer, Handle);
        gl.BufferSubData<T>(GLEnum.ArrayBuffer, (int)(offset * sizeof(T)), (uint)(data.Length * sizeof(T)), data);
        gl.BindBuffer(GLEnum.ArrayBuffer, 0);
    }

    public void SetData<T>(T* data, int length, uint offset = 0) where T : unmanaged
    {
        GL gl = Context.GL;

        gl.BindBuffer(GLEnum.ArrayBuffer, Handle);
        gl.BufferSubData(GLEnum.ArrayBuffer, (int)(offset * sizeof(T)), (uint)(length * sizeof(T)), data);
        gl.BindBuffer(GLEnum.ArrayBuffer, 0);
    }

    public T[] GetData<T>(int length) where T : unmanaged
    {
        GL gl = Context.GL;

        T[] result = new T[length];

        gl.BindBuffer(GLEnum.ArrayBuffer, Handle);
        void* mapBuffer = gl.MapBufferRange(GLEnum.ArrayBuffer, 0, (uint)(length * sizeof(T)), (uint)GLEnum.MapReadBit);

        Span<T> resultSpan = new(mapBuffer, length);
        resultSpan.CopyTo(result);

        gl.UnmapBuffer(GLEnum.ArrayBuffer);
        gl.BindBuffer(GLEnum.ArrayBuffer, 0);

        return result;
    }
}
