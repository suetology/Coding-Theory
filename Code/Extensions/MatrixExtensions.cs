using Math;

namespace Code.Extensions;

/// <summary>
/// Klasė, skirta praplėsti Matrix<T> klasę kodavimo teorijai būdingais metodais.
/// </summary>
public static class MatrixExtensions
{
    /// <summary>
    /// Sukuria standartinio pavidalo generuojančią matricą.
    /// </summary>
    /// <param name="height">Matricos aukštis</param>
    /// <param name="width">Matricos plotis</param>
    /// <returns>Matrica iš Bit elementų</returns>
    public static Matrix<Bit> CreateGeneratorMatrix(int height, int width)
    {
        if (width > height)
        {
            throw new ArgumentException($"Can't create a generator matrix with dimensions {height} x {width}");
        }

        var identityMatrix = Matrix<Bit>.CreateIdentityMatrix(height);

        if (width == height)
        {
            return identityMatrix;
        }

        var rightPartMatrix = new Matrix<Bit>(height, width - height);

        for (var y = 0; y < rightPartMatrix.Height; y++) 
        {
            for (var x = 0; x < rightPartMatrix.Width; x++)
            {
                var value = new Random().Next(0, 2);

                rightPartMatrix[y, x] = new Bit(value);
            }
        }

        var generatorMatrix = Matrix<Bit>.CombineRows(identityMatrix, rightPartMatrix);

        return generatorMatrix;
    }

    /// <summary>
    /// Sukuria kodo kontrolinę matricą.
    /// </summary>
    /// <param name="generatorMatrix">Standartinio pavidalo generuojančia matrica</param>
    /// <returns>Kontrolinė matrica iš Bit elementų</returns>
    public static Matrix<Bit> CreateParityCheckMatrix(this Matrix<Bit> generatorMatrix)
    {
        if (generatorMatrix.Height > generatorMatrix.Width) 
        {
            throw new ArgumentException(
                $"Can't create a parity check matrix from generator matrix with dimensions {generatorMatrix.Height} x {generatorMatrix.Width}");
        }

        if (generatorMatrix.Height == generatorMatrix.Width)
        {
            return Matrix<Bit>.CreateIdentityMatrix(generatorMatrix.Height);
        }

        var matrixRightPart = generatorMatrix.ExtractSubmatrix(0, generatorMatrix.Height, generatorMatrix.Height, generatorMatrix.Width - generatorMatrix.Height);
        var transposedRightPart = matrixRightPart.Transpose();
        var identityMatrix = Matrix<Bit>.CreateIdentityMatrix(transposedRightPart.Height);
        var parityCheckMatrix = Matrix<Bit>.CombineRows(transposedRightPart, identityMatrix);

        return parityCheckMatrix;
    }
}