using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Contracts.GameObjects;
using Triangle.Core.Helpers;

namespace Triangle.Core.GameObjects;

public class TrCamera(string name) : TrGameObject(name)
{
    private float fov = MathHelper.DegreesToRadians(45.0f);

    public int Width { get; set; }

    public int Height { get; set; }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(fov);
        set
        {
            fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
        }
    }

    public float Near { get; set; } = 0.1f;

    public float Far { get; set; } = 1000.0f;

    public float Speed { get; set; } = 2.0f;

    public float Sensitivity { get; set; } = 0.1f;

    public Matrix4X4<float> Projection => Matrix4X4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, Near, Far);

    protected override void OtherPropertyEditor()
    {
        ImGui.Text("Camera");
        ImGui.Separator();

        float cameraSpeed = Speed;
        ImGui.SliderFloat("Speed", ref cameraSpeed, 0.1f, 10.0f);
        Speed = cameraSpeed;

        float cameraSensitivity = Sensitivity;
        ImGui.SliderFloat("Sensitivity", ref cameraSensitivity, 0.1f, 1.0f);
        Sensitivity = cameraSensitivity;
    }

    public void DecomposeView(Matrix4X4<float> view)
    {
        Matrix4X4.Decompose(view.Invert(), out Vector3D<float> _, out Quaternion<float> rotation, out Vector3D<float> position);

        Transform.Position = position;
        Transform.Rotation = rotation;
    }

    protected override void Destroy(bool disposing = false)
    {

    }
}
