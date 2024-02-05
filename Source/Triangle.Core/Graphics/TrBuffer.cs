using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Enums;
using Triangle.Core.Exceptions;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrBuffer(TrContext context, TrBufferTarget bufferTarget, TrBufferUsage bufferUsage, uint length = 1) : TrGraphics<TrContext>(context)
{
    public uint Length { get; } = length;

    public TrBufferTarget BufferTarget { get; } = bufferTarget;

    public TrBufferUsage BufferUsage { get; } = bufferUsage;

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);
    }
}

public unsafe class TrBuffer<TDataType> : TrBuffer where TDataType : unmanaged
{
    public TrBuffer(TrContext context, TrBufferTarget bufferTarget, TrBufferUsage bufferUsage, uint length = 1) : base(context, bufferTarget, bufferUsage, length)
    {
        GL gl = Context.GL;

        Handle = gl.GenBuffer();

        gl.BindBuffer(BufferTarget.ToGL(), Handle);
        gl.BufferData(BufferTarget.ToGL(), (uint)(Length * sizeof(TDataType)), null, BufferUsage.ToGL());
        gl.BindBuffer(BufferTarget.ToGL(), 0);
    }

    public void SetData(TDataType[] data, uint offset = 0)
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

    public void SetData(TDataType data, uint offset = 0)
    {
        GL gl = Context.GL;

        gl.BindBuffer(BufferTarget.ToGL(), Handle);
        gl.BufferSubData(BufferTarget.ToGL(), (int)(offset * sizeof(TDataType)), (uint)sizeof(TDataType), &data);
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
