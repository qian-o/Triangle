using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public class TrTexture : TrGraphics<TrContext>
{
    internal TrTexture(TrContext context) : base(context)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        throw new NotImplementedException();
    }

    protected override void Initialize()
    {
        throw new NotImplementedException();
    }
}
