using Silk.NET.OpenGL;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public unsafe class TrBuffer<TDataType> : TrGraphics<TrContext> where TDataType : unmanaged
{
    public TrBuffer(TrContext context, uint length = 1) : base(context)
    {
        GL gl = Context.GL;

        Handle = gl.CreateBuffer();
        Length = length;

        gl.NamedBufferStorage(Handle, (uint)(Length * sizeof(TDataType)), null, (uint)GLEnum.DynamicStorageBit);
    }

    public uint Length { get; }

    protected override void Destroy(bool disposing = false)
    {
        GL gl = Context.GL;

        gl.DeleteBuffer(Handle);
    }

    public void SetData(TDataType[] data, uint offset = 0)
    {
        fixed (TDataType* dataPtr = data)
        {
            SetData(dataPtr, data.Length, offset);
        }
    }

    public void SetData(TDataType* data, int length, uint offset = 0)
    {
        if (length > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length is greater than buffer length.");
        }

        GL gl = Context.GL;

        gl.NamedBufferSubData(Handle, (int)(offset * sizeof(TDataType)), (uint)(length * sizeof(TDataType)), data);
    }

    public void SetData(TDataType data, uint offset = 0)
    {
        GL gl = Context.GL;

        gl.NamedBufferSubData(Handle, (int)(offset * sizeof(TDataType)), (uint)sizeof(TDataType), &data);
    }

    public void SetData(uint offset, uint size, TDataType* data)
    {
        GL gl = Context.GL;

        gl.NamedBufferSubData(Handle, (int)(offset * sizeof(TDataType)), (uint)(size * sizeof(TDataType)), data);
    }

    public TDataType[] GetData()
    {
        GL gl = Context.GL;

        TDataType[] result = new TDataType[Length];

        void* mapBuffer = gl.MapNamedBuffer(Handle, GLEnum.MapReadBit);

        Span<TDataType> resultSpan = new(mapBuffer, (int)Length);
        resultSpan.CopyTo(result);

        gl.UnmapNamedBuffer(Handle);

        return result;
    }
}
