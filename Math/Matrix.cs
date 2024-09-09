using System.Text;

namespace Math;

public class Matrix<T> 
    where T : IAddable<T>, IMultipliable<T>
{
    public int Height { get; }
    
    public int Width { get; }
    
    private readonly DivisionRingElement[,] _data;

    public Matrix(int height, int width)
    {
        Height = height;
        Width = width;

        _data = new DivisionRingElement[Height, Width];
    }

    public DivisionRingElement this[int row, int column]
    {
        get
        {
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                throw new ArgumentOutOfRangeException("Index out of range");
            }

            return _data[row, column];
        }
        set
        {
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                throw new ArgumentOutOfRangeException("Index out of range");
            }

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

            builder.Append('\n');
        }

        return builder.ToString();
    }
}