using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrTransform
{
    private Vector3D<float> position = Vector3D<float>.Zero;
    private Vector3D<float> scale = Vector3D<float>.One;
    private Quaternion<float> rotation = Quaternion<float>.Identity;

    public Vector3D<float> Position
    {
        get => position;
        set => position = value;
    }

    public Vector3D<float> Scale
    {
        get => scale;
        set => scale = value;
    }

    public Quaternion<float> Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public Vector3D<float> EulerAngles
    {
        get => rotation.ToRotation().RadianToDegree();
        set => rotation = value.DegreeToRadian().ToQuaternion();
    }

    public Vector3D<float> Right => Vector3D.Transform(TrContext.Right, rotation);

    public Vector3D<float> Up => Vector3D.Transform(TrContext.Up, rotation);

    public Vector3D<float> Forward => Vector3D.Transform(TrContext.Forward, rotation);

    public Matrix4X4<float> Model => Matrix4X4.CreateScale(scale) * Matrix4X4.CreateFromQuaternion(rotation) * Matrix4X4.CreateTranslation(position);

    public void Translate(Vector3D<float> translation, TrSpace relativeTo = TrSpace.Local)
    {
        if (relativeTo == TrSpace.World)
        {
            position += translation;
        }
        else
        {
            position += TransformDirection(translation);
        }
    }

    public void Rotate(Vector3D<float> eulerAngles)
    {
        Quaternion<float> quaternion = eulerAngles.DegreeToRadian().ToQuaternion();

        rotation *= Quaternion<float>.Inverse(rotation) * quaternion * rotation;
    }

    public Vector3D<float> TransformDirection(Vector3D<float> translation)
    {
        return Vector3D.Transform(translation, rotation);
    }
}
