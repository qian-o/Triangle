using Common;
using Common.Contracts;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;

namespace ForwardRendering.Applications;

public class Application1 : IApplication
{
    private IWindow _window = null!;
    private TrContext _context = null!;
    private Camera _camera = null!;

    public void Initialize(IWindow window, TrContext context, Camera camera)
    {
        _window = window;
        _context = context;
        _camera = camera;
    }

    public void Update(double deltaSeconds)
    {
    }

    public void Render(double deltaSeconds)
    {
    }
    public void ImGui()
    {
    }

    public void Resize(Vector2D<int> size)
    {
    }

    public void Destroy()
    {
    }
}
