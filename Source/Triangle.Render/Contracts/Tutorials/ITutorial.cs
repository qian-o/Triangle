using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Tutorials;

public interface ITutorial : IDisposable
{
    TrScene Scene { get; }

    void Update(double deltaSeconds);

    void Render(double deltaSeconds);

    void ImGuiRender();
}
