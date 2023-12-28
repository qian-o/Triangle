using Silk.NET.OpenGLES;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrBuffer<TDataType> : TrGraphics<TrContext> where TDataType : unmanaged
{
    public TrBuffer(TrContext context, TrBufferTarget bufferTarget, TrBufferUsage bufferUsage, uint length) : base(context)
    {
        Length = length;
        BufferTarget = bufferTarget;
        BufferUsage = bufferUsage;

        GL gl = Context.GL;

        Handle = gl.GenBuffer();

        gl.BindBuffer(BufferTarget.ToGL(), Handle);
        gl.BufferData(BufferTarget.ToGL(), (uint)(Length * sizeof(TDataType)), null, BufferUsage.ToGL());
        gl.BindBuffer(BufferTarget.ToGL(), 0);
    }

    public uint Length { get; }

    public TrBufferTarget BufferTarget { get; }

    public TrBufferUsage BufferUsage { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);
    }

    public void SetData([NotNull] TDataType[] data, uint offset = 0)
    {
        if (data.Length != Length)
        {
            throw new TrException("数据长度必须等于缓冲区长度。");
        }

        fixed (TDataType* dataPtr = data)
        {
            SetData(dataPtr, offset);
        }
    }

    public void SetData(TDataType* data, uint offset = 0)
    {
        GL gl = Context.GL;

        gl.BindBuffer(BufferTarget.ToGL(), Handle);
        gl.BufferSubData(BufferTarget.ToGL(), (int)(offset * sizeof(TDataType)), (uint)(Length * sizeof(TDataType)), data);
        gl.BindBuffer(BufferTarget.ToGL(), 0);
    }

    public TDataType[] GetData()
    {
        GL gl = Context.GL;

        TDataType[] result = new TDataType[Length];

        gl.BindBuffer(BufferTarget.ToGL(), Handle);
        void* mapBuffer = gl.MapBufferRange(BufferTarget.ToGL(), 0, (uint)(Length * sizeof(TDataType)), (uint)GLEnum.MapReadBit);

        Span<TDataType> resultSpan = new(mapBuffer, (int)Length);
        resultSpan.CopyTo(result);

        gl.UnmapBuffer(BufferTarget.ToGL());
        gl.BindBuffer(BufferTarget.ToGL(), 0);

        return result;
    }
}
