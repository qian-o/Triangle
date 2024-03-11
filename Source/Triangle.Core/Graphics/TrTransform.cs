using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;

namespace Triangle.Core.Graphics;

public partial class TrTransform
{
    private Quaternion<float> rotation = Quaternion<float>.Identity;
    private Vector3D<float> eulerAngles = Vector3D<float>.Zero;

    public Vector3D<float> Position { get; set; } = Vector3D<float>.Zero;

    public Vector3D<float> Scale { get; set; } = Vector3D<float>.One;

    public Quaternion<float> Rotation
    {
        get => rotation;
        set { rotation = value; eulerAngles = value.ToEulerAngles().RadianToDegree(); }
    }

    public Vector3D<float> EulerAngles
    {
        get => eulerAngles;
        set { eulerAngles = value; rotation = value.DegreeToRadian().ToQuaternion(); }
    }

    public Vector3D<float> Right => Vector3D.Transform(DefaultRight, Rotation);

    public Vector3D<float> Up => Vector3D.Transform(DefaultUp, Rotation);

    public Vector3D<float> Forward => Vector3D.Transform(DefaultForward, Rotation);

    public Matrix4X4<float> Model => Matrix4X4.CreateScale(Scale) * Matrix4X4.CreateFromQuaternion(Rotation) * Matrix4X4.CreateTranslation(Position);

    public void Translate(Vector3D<float> translation, TrSpace relativeTo = TrSpace.Local)
    {
        if (relativeTo == TrSpace.World)
        {
            Position += translation;
        }
        else
        {
            Position += TransformDirection(translation);
        }
    }

    public void Rotate(Vector3D<float> eulerAngles)
    {
        EulerAngles += eulerAngles;
    }

    public void Rotate(Quaternion<float> rotation)
    {
        Rotation *= rotation;
    }

    public void Scaled(Vector3D<float> scale)
    {
        Scale *= scale;
    }

    public void Matrix(Matrix4X4<float> matrix)
    {
        Matrix4X4.Decompose(matrix, out Vector3D<float> scale, out Quaternion<float> rotation, out Vector3D<float> translation);

        Scale = scale;
        Rotation = rotation;
        Position = translation;
    }

    public Vector3D<float> TransformDirection(Vector3D<float> translation)
    {
        return Vector3D.Transform(translation, Rotation);
    }
}

public partial class TrTransform
{
    public static Vector3D<float> DefaultZero => Vector3D<float>.Zero;

    public static Vector3D<float> DefaultOne => Vector3D<float>.One;

    public static Vector3D<float> DefaultRight => Vector3D<float>.UnitX;

    public static Vector3D<float> DefaultLeft => -Vector3D<float>.UnitX;

    public static Vector3D<float> DefaultUp => Vector3D<float>.UnitY;

    public static Vector3D<float> DefaultDown => -Vector3D<float>.UnitY;

    public static Vector3D<float> DefaultForward => -Vector3D<float>.UnitZ;

    public static Vector3D<float> DefaultBackward => Vector3D<float>.UnitZ;
}