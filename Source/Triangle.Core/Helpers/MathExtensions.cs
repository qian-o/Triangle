using Silk.NET.Maths;

namespace Triangle.Core.Helpers;

public static class MathExtensions
{
    public static Matrix4X4<T> Invert<T>(this Matrix4X4<T> value) where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        Matrix4X4.Invert(value, out Matrix4X4<T> result);

        return result;
    }
}
