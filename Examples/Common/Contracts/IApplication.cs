using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Graphics;

namespace Common.Contracts;

public interface IApplication : IDisposable
{
    void Initialize(IWindow window, TrContext context, Camera camera);

    void Update(double deltaSeconds);

    void Render(TrFrame frame, double deltaSeconds);

    void DrawImGui();

    void WindowResize(Vector2D<int> size);

    void FramebufferResize(Vector2D<int> size);
}
