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
}
