using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gl) : Disposable
{
    public GL GL { get; } = gl;

    protected override void Destroy(bool disposing = false)
    {
    }

    public void Clear()
    {
        Clear(Vector4D<float>.UnitW);
    }

    public void Clear(Vector4D<float> color)
    {
        GL.ColorMask(true, true, true, true);
        GL.DepthMask(true);
        GL.StencilMask(0xFF);

        GL.ClearColor(color.X, color.Y, color.Z, color.W);
        GL.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);
    }
}