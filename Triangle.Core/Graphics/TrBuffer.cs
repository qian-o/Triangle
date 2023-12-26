using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public class TrBuffer(TrContext context) : TrGraphics<TrContext>(context)
{
    protected override void Initialize()
    {
        throw new NotImplementedException();
    }

    protected override void Destroy(bool disposing = false)
    {
        throw new NotImplementedException();
    }
}
