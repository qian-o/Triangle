using System.Collections.Concurrent;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gl) : Disposable
{
    public const int MaxExecutionCount = 10;

    private readonly ConcurrentQueue<Action> _actions = new();

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

    public void Enqueue(Action action)
    {
        _actions.Enqueue(action);
    }

    public void Execute()
    {
        for (var i = 0; i < MaxExecutionCount; i++)
        {
            if (_actions.TryDequeue(out Action? action))
            {
                action();
            }
            else
            {
                break;
            }
        }
    }
}