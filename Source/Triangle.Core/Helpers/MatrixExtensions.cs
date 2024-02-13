using Silk.NET.Maths;

namespace Triangle.Core.Helpers;

public static class MatrixExtensions
{
    public static float[] ToArray(this Matrix4X4<float> matrix, bool transpose = false)
    {
        matrix = transpose ? Matrix4X4.Transpose(matrix) : matrix;

        float[] array =
        [
            matrix.M11,
            matrix.M12,
            matrix.M13,
            matrix.M14,
            matrix.M21,
            matrix.M22,
            matrix.M23,
            matrix.M24,
            matrix.M31,
            matrix.M32,
            matrix.M33,
            matrix.M34,
            matrix.M41,
            matrix.M42,
            matrix.M43,
            matrix.M44
        ];

        return array;
    }

    public static Matrix4X4<float> ToMatrix(this float[] array, bool transpose = false)
    {
        if (array.Length != 16)
        {
            throw new ArgumentException("Array must contain 16 elements");
        }

        Matrix4X4<float> matrix = new()
        {
            M11 = array[0],
            M12 = array[1],
            M13 = array[2],
            M14 = array[3],
            M21 = array[4],
            M22 = array[5],
            M23 = array[6],
            M24 = array[7],
            M31 = array[8],
            M32 = array[9],
            M33 = array[10],
            M34 = array[11],
            M41 = array[12],
            M42 = array[13],
            M43 = array[14],
            M44 = array[15]
        };

        return transpose ? Matrix4X4.Transpose(matrix) : matrix;
    }

    public static void DecomposeLookAt(this Matrix4X4<float> matrix, out Vector3D<float> cameraPosition, out Vector3D<float> cameraTarget, out Vector3D<float> cameraUpVector)
    {
        Matrix4X4.Invert(matrix, out Matrix4X4<float> inverseMatrix);

        cameraPosition = new Vector3D<float>(inverseMatrix.M41, inverseMatrix.M42, inverseMatrix.M43);
        cameraTarget = cameraPosition - new Vector3D<float>(inverseMatrix.M31, inverseMatrix.M32, inverseMatrix.M33);
        cameraUpVector = new Vector3D<float>(inverseMatrix.M21, inverseMatrix.M22, inverseMatrix.M23);
    }
}
