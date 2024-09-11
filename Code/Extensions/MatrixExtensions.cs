using Math;

namespace Code.Extensions;

public static class MatrixExtensions
{
    public static Matrix<Bit> CreateGeneratorMatrix(int height, int width)
    {
        return null;
    }

    public static bool IsValidGeneratorMatrix(this Matrix<Bit> matrix)
    {
        return true;
    }

    public static void RowReduce(this Matrix<Bit> matrix)
    {
        for (var i = 0; i < matrix.Height; i++)
        {
            if (matrix[i, i] == Bit.Zero())
            {
                for (var j = i + 1; j < matrix.Height; j++)
                {
                    if (matrix[j, i] == Bit.One())
                    {
                        matrix.SwapRows(i, j);
                        
                        break;
                    }
                }
            }

            if (matrix[i, i] == Bit.Zero())
            {
                continue;
            }

            for (var j = 0; j < matrix.Height; j++)
            {
                if (i != j && matrix[j, i] == Bit.One())
                {
                    for (var k = 0; k < matrix.Width; k++)
                    {
                        matrix[j, k] = matrix[j, k] + matrix[i, k];
                    }
                }
            }
        }
    }

    public static Matrix<Bit> CreateParityCheckMatrix(this Matrix<Bit> matrix)
    {
        var identityMatrix = Matrix<Bit>.CreateIdentityMatrix(matrix.Height);
        var zeroMatrix = new Matrix<Bit>(matrix.Height, matrix.Width - matrix.Height);

        var combinedMatrix = Matrix<Bit>.CombineRows(matrix, Matrix<Bit>.CombineRows(identityMatrix, zeroMatrix));

        combinedMatrix.RowReduce();

        var parityCheckMatrix = combinedMatrix.Extract(0, matrix.Width, matrix.Height, matrix.Width);

        Console.WriteLine(combinedMatrix);

        var mul = parityCheckMatrix * matrix.Transpose();

        Console.WriteLine(mul);

        return matrix;
    }
}