using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;

namespace Common.Contracts;

public interface IApplication
{
    void Initialize(IWindow window, TrContext context, Camera camera);

    void Update(double deltaSeconds);

    void Render(double deltaSeconds);

    void Resize(Vector2D<int> size);

    void ImGui();

    void Destroy();
}
