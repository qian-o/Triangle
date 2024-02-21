using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

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

    public float Near { get; set; } = 0.3f;

    public float Far { get; set; } = 1000.0f;

    public Matrix4X4<float> View => Matrix4X4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

    public Matrix4X4<float> Projection => Matrix4X4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, Near, Far);

    public void DecomposeView(Matrix4X4<float> view)
    {
        Matrix4X4.Decompose(view.Invert(), out Vector3D<float> _, out Quaternion<float> rotation, out Vector3D<float> position);

        Transform.Position = position;
        Transform.Rotation = rotation;
    }
}
