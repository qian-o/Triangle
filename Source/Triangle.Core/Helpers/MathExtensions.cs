using Silk.NET.Maths;

namespace Triangle.Core.Helpers;

public static class MathExtensions
{
    public static Matrix4X4<T> Invert<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        Matrix4X4.Invert(value, out Matrix4X4<T> result);

        return result;
    }

    public static Vector3D<float> RadianToDegree(this Vector3D<float> value)
    {
        return value * 180.0f / MathF.PI;
    }

    public static Vector3D<float> DegreeToRadian(this Vector3D<float> value)
    {
        return value * MathF.PI / 180.0f;
    }

    public static Vector3D<float> ToEulerAngles(this Quaternion<float> rotation)
    {
        Vector3D<float> angles = new();

        // roll / x
        float sinr_cosp = 2.0f * (rotation.W * rotation.X + rotation.Y * rotation.Z);
        float cosr_cosp = 1.0f - 2.0f * (rotation.X * rotation.X + rotation.Y * rotation.Y);
        angles.X = MathF.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        float sinp = 2.0f * (rotation.W * rotation.Y - rotation.Z * rotation.X);
        if (MathF.Abs(sinp) >= 1.0f)
        {
            angles.Y = MathF.CopySign(MathF.PI / 2.0f, sinp);
        }
        else
        {
            angles.Y = MathF.Asin(sinp);
        }

        // yaw / z
        float siny_cosp = 2.0f * (rotation.W * rotation.Z + rotation.X * rotation.Y);
        float cosy_cosp = 1.0f - 2.0f * (rotation.Y * rotation.Y + rotation.Z * rotation.Z);
        angles.Z = MathF.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }

    public static Quaternion<float> ToQuaternion(this Vector3D<float> eulerAngles)
    {
        float cy = MathF.Cos(eulerAngles.Z * 0.5f);
        float sy = MathF.Sin(eulerAngles.Z * 0.5f);
        float cp = MathF.Cos(eulerAngles.Y * 0.5f);
        float sp = MathF.Sin(eulerAngles.Y * 0.5f);
        float cr = MathF.Cos(eulerAngles.X * 0.5f);
        float sr = MathF.Sin(eulerAngles.X * 0.5f);

        return new Quaternion<float>
        {
            W = (cr * cp * cy) + (sr * sp * sy),
            X = (sr * cp * cy) - (cr * sp * sy),
            Y = (cr * sp * cy) + (sr * cp * sy),
            Z = (cr * cp * sy) - (sr * sp * cy)
        };
    }
}
