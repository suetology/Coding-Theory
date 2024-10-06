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

    public static IList<Matrix<Bit>> GetCodeWords(this Matrix<Bit> matrix)
    {
        var accumulator = (IList<Matrix<Bit>> codeWords, int position) => 
        {
            if (position >= matrix.Height)
            {
                return;
            }

            
        };

        return accumulator.
    }

    public static void RowReduce(this Matrix<Bit> matrix)
    {
        /*for (var i = 0; i < matrix.Height; i++)


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
        }*/

        int i = 0, j = 0; // row and column indices

        // Main loop for row reduction
        while (i < matrix.Height && j < matrix.Width)
        {
            // Step 1: Find a pivot in column j
            int pivotRow = i;
            while (pivotRow < matrix.Height && matrix[pivotRow, j] == Bit.Zero())
            {
                pivotRow++;
            }

            // If no pivot found in column j, move to the next column
            if (pivotRow == matrix.Height)
            {
                j++;
                continue;
            }

            // Step 2: Swap the current row i with the pivot row
            if (pivotRow != i)
            {
                matrix.SwapRows(i, pivotRow);
                //SwapRows(matrix, i, pivotRow, matrix.Width);
            }

            // Step 3: Eliminate below the pivot (make all entries below the pivot in column j zero)
            for (int r = i + 1; r < matrix.Height; r++)
            {
                if (matrix[r, j] == Bit.One())
                {
                    for (var x = 0; x < matrix.Width; x++)
                    {
                        matrix[r, x] += matrix[i, x];
                    }
                    
                    //AddRows(matrix, i, r, matrix.Width); // Add row i to row r (XOR operation)
                }
            }

            // Move to the next row and column
            i++;
            j++;
        }

        // Optional: Reduce above the pivots (for RREF)
        for (int row = matrix.Height - 1; row >= 0; row--)
        {
            for (int col = 0; col < matrix.Width; col++)
            {
                if (matrix[row, col] == Bit.One())
                {
                    for (int r = row - 1; r >= 0; r--)
                    {
                        if (matrix[r, col] == Bit.One())
                        {
                            for (var x = 0; x < matrix.Width; x++)
                            {
                                matrix[r, x] += matrix[row, x];
                            }

                            //AddRows(matrix, row, r, matrix.Width);
                        }
                    }
                    break;
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

        Console.WriteLine(combinedMatrix);

        var parityCheckMatrix = combinedMatrix.Extract(0, matrix.Width, matrix.Height, matrix.Width);

        var mul = parityCheckMatrix * matrix.Transpose();

        Console.WriteLine(mul);

        return matrix;
    }
}