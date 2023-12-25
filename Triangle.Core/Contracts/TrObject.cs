namespace Triangle.Core.Contracts;

public abstract class TrObject : IDisposable
{
    private bool disposedValue;

    ~TrObject()
    {
        Dispose(disposing: false);
    }

    protected abstract void Destroy();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (!disposing)
            {
                Destroy();
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
