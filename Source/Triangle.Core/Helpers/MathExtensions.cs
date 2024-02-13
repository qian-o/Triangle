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

    public static void ToEulerAngles(this Quaternion<float> rotation, out float pitch, out float yaw, out float roll)
    {
        float sqw = rotation.W * rotation.W;
        float sqx = rotation.X * rotation.X;
        float sqy = rotation.Y * rotation.Y;
        float sqz = rotation.Z * rotation.Z;
        float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
        float test = rotation.X * rotation.W - rotation.Y * rotation.Z;

        if (test > 0.49975f * unit)
        {   
            // singularity at north pole
            yaw = 2f * MathF.Atan2(rotation.Y, rotation.X);
            pitch = MathF.PI / 2f;
            roll = 0;
            return;
        }
        if (test < -0.49975f * unit)
        {   
            // singularity at south pole
            yaw = -2f * MathF.Atan2(rotation.Y, rotation.X);
            pitch = -MathF.PI / 2f;
            roll = 0;
            return;
        }

        Quaternion<float> q1 = new(rotation.W, rotation.Z, rotation.X, rotation.Y);

        yaw = 1 * MathF.Atan2(2f * (q1.X * q1.W + q1.Y * q1.Z), 1f - 2f * (q1.Z * q1.Z + q1.W * q1.W));   // Yaw
        pitch = 1 * MathF.Asin(2f * (q1.X * q1.Z - q1.W * q1.Y));                                         // Pitch
        roll = 1 * MathF.Atan2(2f * (q1.X * q1.Y + q1.Z * q1.W), 1f - 2f * (q1.Y * q1.Y + q1.Z * q1.Z));  // Roll
    }
}
