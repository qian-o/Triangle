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
        double sinr_cosp = 2 * (rotation.W * rotation.X + rotation.Y * rotation.Z);
        double cosr_cosp = 1 - 2 * (rotation.X * rotation.X + rotation.Y * rotation.Y);
        angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

        // pitch / y
        double sinp = 2 * (rotation.W * rotation.Y - rotation.Z * rotation.X);
        if (Math.Abs(sinp) >= 1)
        {
            angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
        }
        else
        {
            angles.Y = (float)Math.Asin(sinp);
        }

        // yaw / z
        double siny_cosp = 2 * (rotation.W * rotation.Z + rotation.X * rotation.Y);
        double cosy_cosp = 1 - 2 * (rotation.Y * rotation.Y + rotation.Z * rotation.Z);
        angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

        return angles;
    }

    public static Quaternion<float> ToQuaternion(this Vector3D<float> eulerAngles)
    {
        float cy = (float)Math.Cos(eulerAngles.Z * 0.5);
        float sy = (float)Math.Sin(eulerAngles.Z * 0.5);
        float cp = (float)Math.Cos(eulerAngles.Y * 0.5);
        float sp = (float)Math.Sin(eulerAngles.Y * 0.5);
        float cr = (float)Math.Cos(eulerAngles.X * 0.5);
        float sr = (float)Math.Sin(eulerAngles.X * 0.5);

        return new Quaternion<float>
        {
            W = (cr * cp * cy + sr * sp * sy),
            X = (sr * cp * cy - cr * sp * sy),
            Y = (cr * sp * cy + sr * cp * sy),
            Z = (cr * cp * sy - sr * sp * cy)
        };
    }

    public static Vector3D<float> NormalizeEulerAngleDegrees(this Vector3D<float> angle)
    {
        float normalizedX = angle.X % 360.0f;
        float normalizedY = angle.Y % 360.0f;
        float normalizedZ = angle.Z % 360.0f;

        if (normalizedX < 0)
        {
            normalizedX += 360.0f;
        }

        if (normalizedY < 0)
        {
            normalizedY += 360.0f;
        }

        if (normalizedZ < 0)
        {
            normalizedZ += 360.0f;
        }

        return new(normalizedX, normalizedY, normalizedZ);
    }
}
