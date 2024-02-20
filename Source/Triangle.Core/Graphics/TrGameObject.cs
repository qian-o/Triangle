namespace Triangle.Core.Graphics;

public abstract class TrGameObject(string name)
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly TrTransform _transform = new();

    public Guid Id => _id;

    public string Name => name;

    public TrTransform Transform => _transform;
}
