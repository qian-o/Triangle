using Common;
using ForwardRendering.Applications;

namespace ForwardRendering;

internal sealed class Program
{
    static void Main(string[] _)
    {
        using RenderHost<TutorialApplication> renderHost = new("Forward Rendering");
        renderHost.Run();
    }
}
