using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gl) : Disposable
{
    public static Vector3D<float> Zero => Vector3D<float>.Zero;

    public static Vector3D<float> One => Vector3D<float>.One;

    public static Vector3D<float> Right => Vector3D<float>.UnitX;

    public static Vector3D<float> Left => -Vector3D<float>.UnitX;

    public static Vector3D<float> Up => Vector3D<float>.UnitY;

    public static Vector3D<float> Down => -Vector3D<float>.UnitY;

    public static Vector3D<float> Forward => -Vector3D<float>.UnitZ;

    public static Vector3D<float> Backward => Vector3D<float>.UnitZ;

    public GL GL { get; } = gl;

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            // TODO: 清理扩展资源。
        }
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