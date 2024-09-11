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

    public static Matrix<Bit> CreateSystematicFormMatrix(this Matrix<Bit> matrix)
    {
        var systematicFormMatrix = new Matrix<Bit>(matrix);

        for (var i = 0; i < systematicFormMatrix.Height; i++)
        {
            if (systematicFormMatrix[i, i] == Bit.Zero())
            {
                for (var j = i + 1; j < systematicFormMatrix.Height; j++)
                {
                    if (systematicFormMatrix[j, i] == Bit.One())
                    {
                        systematicFormMatrix.SwapRows(i, j);
                        
                        break;
                    }
                }
            }

            if (systematicFormMatrix[i, i] == Bit.Zero())
            {
                continue;
            }

            for (var j = 0; j < systematicFormMatrix.Height; j++)
            {
                if (i != j && systematicFormMatrix[j, i] == Bit.One())
                {
                    for (var k = 0; k < systematicFormMatrix.Width; k++)
                    {
                        systematicFormMatrix[j, k] = systematicFormMatrix[j, k] + systematicFormMatrix[i, k];
                    }
                }
            }
        }

        return systematicFormMatrix;
    }

    public static Matrix<Bit> CreateParityCheckMatrix(this Matrix<Bit> matrix)
    {
        var systematicFormMatrix = new Matrix<Bit>(matrix);

        return systematicFormMatrix;
    }
}