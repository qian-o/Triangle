using Silk.NET.Maths;
using Triangle.Core.Helpers;
using Triangle.Core.Materials;

namespace Triangle.Core.GameObjects;

public class TrPointLight(TrContext context, TrCamera camera, string name) : TrModel(name, [context.CreateSphere()], new PointLightMat(context))
{
    public Vector3D<float> Color { get; set; } = new(1.0f, 0.9569f, 0.8392f);

    public float Intensity { get; set; } = 1.0f;

    public float Range { get; set; } = 10.0f;

    public override void Render()
    {
        Render([camera, Color, Intensity, Range]);
    }

    protected override void OtherPropertyEditor()
    {
        Vector3D<float> c = Color;
        float i = Intensity;
        float r = Range;

        ImGuiHelper.ColorEdit3("Color", ref c);
        ImGuiHelper.DragFloat("Intensity", ref i, 0.1f, 0.0f, 10.0f);
        ImGuiHelper.DragFloat("Range", ref r, 0.1f, 0.0f, 100.0f);

        Color = c;
        Intensity = i;
        Range = r;
    }
}
