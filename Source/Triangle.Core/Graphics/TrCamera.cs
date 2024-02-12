using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrCamera
{
    private Vector3D<float> front = -Vector3D<float>.UnitZ;
    private Vector3D<float> up = Vector3D<float>.UnitY;
    private Vector3D<float> right = Vector3D<float>.UnitX;
    private float pitch;
    private float yaw = -MathHelper.PiOver2;
    private float fov = MathHelper.PiOver2;

    public int Width { get; set; }

    public int Height { get; set; }

    public Vector3D<float> Position { get; set; } = new(0.0f, 0.0f, 0.0f);

    public Vector3D<float> Front => front;

    public Vector3D<float> Up => up;

    public Vector3D<float> Right => right;

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(pitch);
        set
        {
            pitch = MathHelper.DegreesToRadians(MathHelper.Clamp(value, -89f, 89f));

            UpdateVectors();
        }
    }

    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(yaw);
        set
        {
            yaw = MathHelper.DegreesToRadians(value);

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

    public Matrix4X4<float> View => Matrix4X4.CreateLookAt(Position, Position + Front, Up);

    public Matrix4X4<float> Projection => Matrix4X4.CreatePerspectiveFieldOfView(fov, (float)Width / Height, Near, Far);

    public void InverseView(Matrix4X4<float> matrix)
    {
        matrix.InverseLookAt(out Vector3D<float> cameraPosition, out Vector3D<float> cameraTarget, out Vector3D<float> _);

        Position = cameraPosition;

        Vector3D<float> front = cameraTarget - cameraPosition;

        pitch = MathF.Asin(front.Y);
        yaw = MathF.Atan2(front.Z, front.X);

        UpdateVectors();
    }

    private void UpdateVectors()
    {
        front.X = MathF.Cos(pitch) * MathF.Cos(yaw);
        front.Y = MathF.Sin(pitch);
        front.Z = MathF.Cos(pitch) * MathF.Sin(yaw);

        front = Vector3D.Normalize(front);

        right = Vector3D.Normalize(Vector3D.Cross(front, Vector3D<float>.UnitY));
        up = Vector3D.Normalize(Vector3D.Cross(right, front));
    }
}
