using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;

namespace Triangle.Core.Contracts.GameObjects;

public abstract class TrGameObject(string name) : Disposable
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

            Vector3D<float> t = Transform.Position;
            Vector3D<float> r = Transform.EulerAngles;
            Vector3D<float> s = Transform.Scale;

            ImGuiHelper.DragFloat3("Translation", ref t, 0.01f);
            ImGuiHelper.DragFloat3("Rotation", ref r, 0.01f);
            ImGuiHelper.DragFloat3("Scale", ref s, 0.01f);

            Transform.Position = t;
            Transform.EulerAngles = r;
            Transform.Scale = s;

            OtherPropertyEditor();
        }
        ImGui.PopID();
    }

    protected virtual void OtherPropertyEditor()
    {
    }
}
