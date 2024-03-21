namespace Triangle.Core.Contracts;

public abstract class Disposable : IDisposable
{
    private volatile uint _isDisposed;

    ~Disposable()
    {
        Dispose(disposing: false);
    }

    public bool IsDisposed => _isDisposed != 0;

    protected abstract void Destroy(bool disposing = false);

    protected virtual void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
        {
            return;
        }

        Destroy(disposing);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
