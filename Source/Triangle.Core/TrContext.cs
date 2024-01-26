using Silk.NET.OpenGL;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gl) : Disposable
{
    public GL GL { get; } = gl;

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            // TODO: 清理扩展资源。
        }
    }
}