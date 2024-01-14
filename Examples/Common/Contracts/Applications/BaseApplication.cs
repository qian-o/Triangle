using System.Diagnostics.CodeAnalysis;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;

namespace Common.Contracts.Applications;

public abstract class BaseApplication : IApplication
{
    private bool disposedValue;

    ~BaseApplication()
    {
        Dispose(disposing: false);
    }

    public IWindow Window { get; private set; } = null!;

    public IInputContext Input { get; private set; } = null!;

    public TrContext Context { get; private set; } = null!;

    public void Initialize([NotNull] IWindow window, [NotNull] IInputContext input, [NotNull] TrContext context)
    {
        Window = window;
        Input = input;
        Context = context;

        Loaded();
    }

    public abstract void Loaded();

    public abstract void Update(double deltaSeconds);

    public abstract void Render(double deltaSeconds);

    public abstract void ImGuiRender();

    public abstract void WindowResize(Vector2D<int> size);

    protected abstract void Destroy(bool disposing = false);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Destroy(disposing);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
