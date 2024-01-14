using Common.Models;

namespace ForwardRendering.Contracts.Tutorials;

public interface ITutorial : IDisposable
{
    TrScene Scene { get; }

    void Update(double deltaSeconds);

    void Render(double deltaSeconds);
}
