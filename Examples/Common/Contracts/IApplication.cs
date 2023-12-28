using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Graphics;

namespace Common.Contracts;

public interface IApplication : IDisposable
{
    public IWindow Window { get; }

    public TrContext Context { get; }

    void Initialize(IWindow window, TrContext context);

    void Update(double deltaSeconds);

    void Render(Camera camera, TrFrame frame, double deltaSeconds);

    void DrawImGui();

    void Resize(Vector2D<int> size);
}
