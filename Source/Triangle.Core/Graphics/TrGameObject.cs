using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;

namespace Triangle.Core.Graphics;

public abstract class TrGameObject(string name)
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly TrTransform _transform = new();

    private Vector4D<byte>? colorId;

    public Guid Id => _id;

    public Vector4D<byte> ColorId => colorId ??= GenerateColor();

    public string Name => name;

    public TrTransform Transform => _transform;

    private Vector4D<byte> GenerateColor()
    {
        byte[] bytes = Id.ToByteArray();

        byte r = (byte)(bytes[0] ^ bytes[4] ^ bytes[8] ^ bytes[12]);
        byte g = (byte)(bytes[1] ^ bytes[5] ^ bytes[9] ^ bytes[13]);
        byte b = (byte)(bytes[2] ^ bytes[6] ^ bytes[10] ^ bytes[14]);

        return new Vector4D<byte>(r, g, b, 255);
    }

    public void PropertyEditor()
    {
        ImGui.PushID(GetHashCode());
        {
            ImGui.Text(Name);
            ImGui.Separator();

            Vector3 t = Transform.Position.ToSystem();
            Vector3 r = Transform.EulerAngles.ToSystem();
            Vector3 s = Transform.Scale.ToSystem();

            ImGui.DragFloat3("Translation", ref t, 0.01f);
            ImGui.SliderFloat3("Rotation", ref r, -360.0f, 360.0f);
            ImGui.DragFloat3("Scale", ref s, 0.01f);

            Transform.Position = t.ToGeneric();
            Transform.EulerAngles = r.ToGeneric();
            Transform.Scale = s.ToGeneric();

            OtherPropertyEditor();
        }
        ImGui.PopID();
    }

    protected virtual void OtherPropertyEditor()
    {
    }
}
