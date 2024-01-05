using Common;
using ForwardRendering.Applications;

namespace ForwardRendering;

internal sealed class Program
{
    static void Main(string[] _)
    {
        using Application3 application = new();
        using Rendering rendering = new(application, "Forward Rendering");
        rendering.Run();
    }
}
