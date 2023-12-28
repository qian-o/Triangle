using Common;
using ForwardRendering.Applications;

namespace ForwardRendering;

internal sealed class Program
{
    static void Main(string[] _)
    {
        using Rendering rendering = new(new Application1());
        rendering.Run();
    }
}
