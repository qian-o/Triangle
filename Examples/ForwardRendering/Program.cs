using Common;
using ForwardRendering.Applications;
using Triangle.Core.Helpers;

namespace ForwardRendering;

internal sealed class Program
{
    static void Main(string[] _)
    {
        ShadercHelper.CompileSpirv("Resources/Shaders");

        using RenderHost<TutorialApplication> renderHost = new("Forward Rendering");
        renderHost.Run();
    }
}
