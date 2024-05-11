using Silk.NET.Assimp;
using Silk.NET.Maths;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public unsafe class TrMaterialProperty : TrGraphics<TrContext>
{
    public TrMaterialProperty(TrContext context, Material* material) : base(context)
    {
        for (uint i = 0; i < material->MNumProperties; i++)
        {
            MaterialProperty* property = material->MProperties[i];

            if (property->MKey.AsString == Assimp.MaterialColorDiffuseBase)
            {
                DiffuseColor = ((nint)property->MData).ToVector4D<float>();
            }
            else if (property->MKey.AsString == Assimp.MaterialColorSpecularBase)
            {
                SpecularColor = ((nint)property->MData).ToVector4D<float>();
            }
        }
    }

    public Vector4D<float> DiffuseColor { get; }

    public Vector4D<float> SpecularColor { get; }

    protected override void Destroy(bool disposing = false)
    {
    }
}
