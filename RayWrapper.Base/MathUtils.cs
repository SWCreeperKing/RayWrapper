using System.Numerics;

namespace RayWrapper.Base;

public class MathUtils
{
    public static float[] MatrixToBuffer(Matrix4x4 matrix)
    {
        return new[]
        {
            matrix.M11, matrix.M21, matrix.M31, matrix.M41,
            matrix.M12, matrix.M22, matrix.M32, matrix.M42,
            matrix.M13, matrix.M23, matrix.M33, matrix.M43,
            matrix.M14, matrix.M24, matrix.M34, matrix.M44
        };
    }
}