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
        return new(value.X * 180.0f / MathF.PI, value.Y * 180.0f / MathF.PI, value.Z * 180.0f / MathF.PI);
    }

    public static Vector3D<float> DegreeToRadian(this Vector3D<float> value)
    {
        return new(value.X * MathF.PI / 180.0f, value.Y * MathF.PI / 180.0f, value.Z * MathF.PI / 180.0f);
    }

    public static Vector3D<float> ToRotation(this Quaternion<float> rotation)
    {
        float yaw = MathF.Atan2(2.0f * (rotation.Y * rotation.W + rotation.X * rotation.Z), 1.0f - 2.0f * (rotation.X * rotation.X + rotation.Y * rotation.Y));
        float pitch = MathF.Asin(2.0f * (rotation.X * rotation.W - rotation.Y * rotation.Z));
        float roll = MathF.Atan2(2.0f * (rotation.X * rotation.Y + rotation.Z * rotation.W), 1.0f - 2.0f * (rotation.X * rotation.X + rotation.Z * rotation.Z));

        // If any nan or inf, set that value to 0
        if (float.IsNaN(yaw) || float.IsInfinity(yaw))
        {
            yaw = 0;
        }

        if (float.IsNaN(pitch) || float.IsInfinity(pitch))
        {
            pitch = 0;
        }

        if (float.IsNaN(roll) || float.IsInfinity(roll))
        {
            roll = 0;
        }

        return new Vector3D<float>(pitch, yaw, roll);
    }

    public static Quaternion<float> ToQuaternion(this Vector3D<float> eulerAngles)
    {
        return Quaternion<float>.CreateFromYawPitchRoll(eulerAngles.Y, eulerAngles.X, eulerAngles.Z);
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
