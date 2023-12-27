using Silk.NET.OpenGLES;
using Triangle.Core.Contracts;

namespace Triangle.Core;

public class TrContext(GL gl) : TrObject
{
    public GL GL { get; } = gl;

    protected override void Destroy(bool disposing = false)
    {
    }
}