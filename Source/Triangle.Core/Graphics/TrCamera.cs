using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrCamera
{
    private Vector3D<float> position = Vector3D<float>.Zero;
    private Vector2D<float> rotation = new(0.0f, MathHelper.DegreesToRadians(-90.0f));
    private float fov = MathHelper.DegreesToRadians(45.0f);
    private Vector3D<float> front = -Vector3D<float>.UnitZ;
    private Vector3D<float> right = Vector3D<float>.UnitX;
    private Vector3D<float> up = Vector3D<float>.UnitY;

    public int Width { get; set; }

    public int Height { get; set; }

    public Vector3D<float> Position
    {
        get => position;
        set
        {
            position = value;

            UpdateVectors();
        }
    }

    public Vector2D<float> Rotation
    {
        get => rotation.RadianToDegree();
        set
        {
            value.X = MathHelper.Clamp(value.X, -89.9f, 89.9f);

            rotation = value.DegreeToRadian();

            UpdateVectors();
        }
    }

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

    public Vector3D<float> Front => front;

    public Vector3D<float> Right => right;

    public Vector3D<float> Up => up;

    public Matrix4X4<float> View => Matrix4X4.CreateLookAt(position, position + front, up);

    public Matrix4X4<float> Projection => Matrix4X4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, Near, Far);

    public void UpdateVectors()
    {
        front.X = MathF.Cos(rotation.X) * MathF.Cos(rotation.Y);
        front.Y = MathF.Sin(rotation.X);
        front.Z = MathF.Cos(rotation.X) * MathF.Sin(rotation.Y);

        front = Vector3D.Normalize(front);

        right = Vector3D.Normalize(Vector3D.Cross(front, Vector3D<float>.UnitY));
        up = Vector3D.Normalize(Vector3D.Cross(right, front));
    }

    public void DecomposeView(Matrix4X4<float> view)
    {
        view.DecomposeLookAt(out Vector3D<float> cameraPosition, out Vector3D<float> cameraTarget, out Vector3D<float> _);

        Vector3D<float> front = cameraTarget - cameraPosition;

        Rotation = new Vector2D<float>(MathF.Asin(front.Y), MathF.Atan2(front.Z, front.X)).RadianToDegree();
        Position = cameraPosition;
    }
}
