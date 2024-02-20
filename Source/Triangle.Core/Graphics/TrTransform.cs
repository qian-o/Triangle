using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public class TrTransform
{
    private Vector3D<float> position = Vector3D<float>.Zero;
    private Vector3D<float> scale = Vector3D<float>.One;
    private Quaternion<float> rotation = Quaternion<float>.Identity;
    private Quaternion<float> localRotation = Quaternion<float>.Identity;

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
        get => rotation.ToRotation().RadianToDegree().NormalizeEulerAngleDegrees();
        set => rotation = value.NormalizeEulerAngleDegrees().DegreeToRadian().ToQuaternion();
    }

    public Quaternion<float> LocalRotation
    {
        get => localRotation;
        set => localRotation = value;
    }

    public Vector3D<float> LocalEulerAngles
    {
        get => localRotation.ToRotation().RadianToDegree().NormalizeEulerAngleDegrees();
        set => localRotation = value.NormalizeEulerAngleDegrees().DegreeToRadian().ToQuaternion();
    }

    public Vector3D<float> Right => Vector3D.Transform(TrContext.Right, localRotation);

    public Vector3D<float> Up => Vector3D.Transform(TrContext.Up, localRotation);

    public Vector3D<float> Forward => Vector3D.Transform(TrContext.Forward, localRotation);

    public Matrix4X4<float> Model => Matrix4X4.CreateScale(scale) * Matrix4X4.CreateFromQuaternion(localRotation) * Matrix4X4.CreateTranslation(position) * Matrix4X4.CreateFromQuaternion(rotation);

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

    public void Rotate(Vector3D<float> eulerAngles, TrSpace relativeTo = TrSpace.Local)
    {
        Quaternion<float> quaternion = eulerAngles.DegreeToRadian().ToQuaternion();

        if (relativeTo == TrSpace.World)
        {
            rotation *= Quaternion<float>.Inverse(rotation) * quaternion * rotation;
        }
        else
        {
            localRotation *= Quaternion<float>.Inverse(localRotation) * quaternion * localRotation;
        }
    }

    public Vector3D<float> TransformDirection(Vector3D<float> translation)
    {
        return Vector3D.Transform(translation, localRotation);
    }
}
