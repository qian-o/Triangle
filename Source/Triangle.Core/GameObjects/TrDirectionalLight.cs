using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Materials;

namespace Triangle.Core.GameObjects;

public class TrDirectionalLight(TrContext context, TrCamera camera, string name) : TrModel(name, context.AssimpParsing(Path.Combine("Resources", "Models", "DirectionalLight.glb")).Meshes, new DirectionalLightMat(context))
{
    public Vector3D<float> Color { get; set; } = new(1.0f, 0.9569f, 0.8392f);

    public Vector3D<float> Direction => Transform.Forward;

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

    protected override void Destroy(bool disposing = false)
    {
        foreach (TrMaterial material in Materials)
        {
            material.Dispose();
        }

        base.Destroy(disposing);
    }
}
