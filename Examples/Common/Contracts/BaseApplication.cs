using Silk.NET.Maths;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Graphics;

namespace Common.Contracts;

public abstract class BaseApplication : IApplication
{
    private bool disposedValue;

    ~BaseApplication()
    {
        Dispose(disposing: false);
    }

    public IWindow Window { get; private set; } = null!;

    public TrContext Context { get; private set; } = null!;

    public Camera Camera { get; private set; } = null!;

    public virtual void Initialize(IWindow window, TrContext context, Camera camera)
    {
        Window = window;
        Context = context;
        Camera = camera;
    }

    public abstract void Update(double deltaSeconds);

    public abstract void Render(TrFrame frame, double deltaSeconds);

    public abstract void DrawImGui();

    public abstract void WindowResize(Vector2D<int> size);

    public abstract void FramebufferResize(Vector2D<int> size);

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
