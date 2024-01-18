using Triangle.App.Models;

namespace Triangle.App.Contracts.Tutorials;

public interface ITutorial : IDisposable
{
    TrScene Scene { get; }

    void Update(double deltaSeconds);

    void Render(double deltaSeconds);

    void ImGuiRender();
}
