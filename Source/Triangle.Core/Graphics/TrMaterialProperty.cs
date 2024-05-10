using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public class TrMaterialProperty(TrContext context) : TrGraphics<TrContext>(context)
{
    protected override void Destroy(bool disposing = false)
    {
    }
}
