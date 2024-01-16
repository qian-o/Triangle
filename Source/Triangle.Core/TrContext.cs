using Silk.NET.OpenGL;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gL) : TrObject
{
    public GL GL { get; } = gL;

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            // TODO: 清理扩展资源。
        }
    }
}