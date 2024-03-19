using Silk.NET.Maths;
using Triangle.Core.Helpers;
using Triangle.Core.Materials;

namespace Triangle.Core.GameObjects;

public class TrAmbientLight(TrContext context, TrCamera camera, string name) : TrModel(name, [context.GetSphere()], new AmbientLightMat(context))
{
    public Vector3D<float> Color { get; set; } = new(0.21176471f, 0.22745098f, 0.25882354f);

    public override void Render()
    {
        Render([camera, Color]);
    }

    protected override void OtherPropertyEditor()
    {
        Vector3D<float> c = Color;

        ImGuiHelper.ColorEdit3("Color", ref c);

        Color = c;
    }
}
