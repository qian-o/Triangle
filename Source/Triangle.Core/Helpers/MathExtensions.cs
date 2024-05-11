using System.Runtime.CompilerServices;
using Silk.NET.Maths;

namespace Triangle.Core.Helpers;

public unsafe static class MathExtensions
{
    public static Vector2D<T> ToVector2D<T>(this nint data) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        T* ptrT = (T*)data;

        return new Vector2D<T>(Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT));
    }

    public static Vector3D<T> ToVector3D<T>(this nint data) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        T* ptrT = (T*)data;

        return new Vector3D<T>(Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT));
    }

    public static Vector4D<T> ToVector4D<T>(this nint data) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        T* ptrT = (T*)data;

        return new Vector4D<T>(Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT++), Unsafe.Read<T>(ptrT));
    }

    public static Matrix4X4<T> Invert<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        Matrix4X4.Invert(value, out Matrix4X4<T> result);

        return result;
    }

    public static Vector4D<byte> ToByte(this Vector4D<float> value)
    {
        return new Vector4D<byte>((byte)(value.X * 255), (byte)(value.Y * 255), (byte)(value.Z * 255), (byte)(value.W * 255));
    }

    public static Vector4D<float> ToSingle(this Vector4D<byte> value)
    {
        return new Vector4D<float>(value.X / 255.0f, value.Y / 255.0f, value.Z / 255.0f, value.W / 255.0f);
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
}
