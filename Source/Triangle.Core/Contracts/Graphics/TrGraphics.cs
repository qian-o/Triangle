namespace Triangle.Core.Contracts.Graphics;

public abstract class TrGraphics<TContext> : Disposable where TContext : Disposable
{
    protected TrGraphics(TContext context)
    {
        Context = context;
    }

    public TContext Context { get; }

    public uint Handle { get; protected set; }
}