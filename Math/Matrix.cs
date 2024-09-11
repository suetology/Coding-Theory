namespace Math;

using System.Text;

public class Matrix<T> 
    where T : INumeric<T>
{
    public int Height { get; }
    
    public int Width { get; }
    
    private readonly T[,] _data;

    public Matrix(int height, int width)
    {
        Height = height;
        Width = width;

        _data = new T[Height, Width];
    }

    public Matrix(Matrix<T> other)
    {
        Height = other.Height;
        Width = other.Width;

        _data = new T[Height, Width];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                _data[y, x] = other[y, x];
            }
        }
    }

    public Matrix<T> Transpose()
    {
        var transpose = new Matrix<T>(Width, Height);

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                transpose[x, y] = this[y, x];
            }
        }

        return transpose;
    }

    public Matrix<T> Extract(int rowStart, int rowCount, int columnStart, int columnCount)
    {
        var extracted = new Matrix<T>(rowCount, columnCount);

        for (var y = rowStart; y < rowStart + rowCount; y++)
        {
            for (var x = columnStart; x < columnStart + columnCount; x++)
            {
                extracted[y, x] = this[rowStart + y, columnStart + x];
            }
        }

        return extracted;
    }

    public void SwapRows(int r1, int r2)
    {
        for (var i = 0; i < Width; i++)
        {
            (this[r1, i], this[r2, i]) = (this[r2, i], this[r1, i]);
        }
    }

    public static Matrix<T> CombineRows(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1.Height != m2.Height)
        {
            throw new ArgumentException("Can't combine matrices with different height");
        }

        var combined = new Matrix<T>(m1.Height, m1.Width + m2.Width);

        for (var y = 0; y < m1.Height; y++)
        {
            for (var x = 0; x < m1.Width; x++)
            {
                combined[y, x] = m1[y, x];
            }
        }

        for (var y = 0; y < m2.Height; y++)
        {
            for (var x = 0; x < m2.Width; x++)
            {
                combined[y, m1.Width + x] = m2[y, x];
            }
        }

        return combined;
    }

    public static Matrix<T> CreateIdentityMatrix(int size)
    {
        var identityMatrix = new Matrix<T>(size, size);

        for (var i = 0; i < size; i++)
        {
            identityMatrix[i, i] = T.One();
        }

        return identityMatrix;
    }

    public T this[int row, int column]
    {
        get
        {
            return _data[row, column];
        }
        set
        {
            _data[row, column] = value;
        }
    }

    public static Matrix<T> operator +(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1.Height != m2.Height || m1.Width != m2.Width)
        {
            throw new ArgumentException("Matrices should have the same dimentions for addition");
        }

        var result = new Matrix<T>(m1.Height, m1.Width);

        for (var y = 0; y < m1.Height; y++)
        {
            for (var x = 0; x < m1.Width; x++)
            {
                result[y, x] = m1[y, x] + m2[y, x];
            }
        }

        return result;
    }

    public static Matrix<T> operator *(Matrix<T> m1, Matrix<T> m2)
    {
        if (m1.Width != m2.Height)
        {
            throw new ArgumentException("Invalid matrix dimensions for multiplication");
        }

        var result = new Matrix<T>(m1.Height, m2.Width);

        for (var i = 0; i < m1.Height; i++)
        {
            for (var j = 0; j < m2.Width; j++)
            {
                for (var k = 0; k < m1.Width; k++)
                {
                    result[i, j] += m1[i, k] * m2[k, j];
                }
            }
        }

        return result;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                builder.Append(_data[y, x] + " ");
            }

            if (y != Height - 1)
            {
                builder.Append('\n');
            }
        }

        return builder.ToString();
    }
}