namespace Math;

using System.Text;

/// <summary>
/// Klasė, skirta darbui su matricomis.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Matrix<T> : IEquatable<Matrix<T>>
    where T : INumeric<T>
{
    public int Height { get; }
    
    public int Width { get; }
    
    private readonly T[,] _data;

    /// <summary>
    /// Sukuria tuščia Matrix objektą.
    /// </summary>
    /// <param name="height">Matricos aukštis</param>
    /// <param name="width">Matricos plotis</param>
    public Matrix(int height, int width)
    {
        Height = height;
        Width = width;

        _data = new T[Height, Width];
    }

    /// <summary>
    /// Sukuria matricos kopija.
    /// </summary>
    /// <param name="other">Originali matrica</param>
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

    /// <summary>
    /// Sukuria naują transponuotą matricą.
    /// </summary>
    /// <returns>Transponuota matrica</returns>
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

    /// <summary>
    /// Pagalbinis metodas, leidžiantis ištraukti tam tikrą dalį iš matricos.
    /// </summary>
    /// <param name="rowStart">Pirmos eilutės pozicija</param>
    /// <param name="columnStart">Pirmo stulpelio pozicija</param>
    /// <param name="height">Eilučių skaičius</param>
    /// <param name="width">Stulpelių skaičius</param>
    /// <returns>Matricos dalis</returns>
    public Matrix<T> ExtractSubmatrix(int rowStart, int columnStart, int height, int width)
    {
        var extracted = new Matrix<T>(height, width);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                extracted[y, x] = this[rowStart + y, columnStart + x];
            }
        }

        return extracted;
    }

    /// <summary>
    /// Pagalbinis metodas, leidžiantis patikrinti, ar matrica yra standartinio pavidalo.
    /// </summary>
    /// <returns>true, kai matrica yra standartinio pavidalo, kitaip - false</returns>
    public bool IsStandartFormMatrix()
    {
        if (Height > Width)
        {
            return false;
        }

        var leftPartMatrix = ExtractSubmatrix(0, 0, Height, Height);

        return leftPartMatrix.IsIdentityMatrix();
    }

    /// <summary>
    /// Pagalbinis metodas, leidžiantis patikrinti, ar matrica yra vienetinė.
    /// </summary>
    /// <returns>true, kai matrica yra vienetinė, kitaip - false</returns>
    public bool IsIdentityMatrix()
    {
        if (Height != Width)
        {
            return false;
        }

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (y == x)
                {
                    if (this[y, x] != T.One())
                    {
                        return false;
                    }
                }
                else
                {
                    if (this[y, x] != T.Zero())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Pagalbinis metodas, apjungiantis dviejų matricų eilutės.
    /// </summary>
    /// <param name="m1">Naujos matricos kairioji dalis</param>
    /// <param name="m2">Naujos matricos dešinioji dalis</param>
    /// <returns>Matrica su apjungtomis eilutėmis</returns>
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

    /// <summary>
    /// Pagalbinis metodas, sukuriantis vienetinę matricą.
    /// </summary>
    /// <param name="size">Vienetinės matricos išmatavimai</param>
    /// <returns>Vienetinė matrica</returns>
    public static Matrix<T> CreateIdentityMatrix(int size)
    {
        var identityMatrix = new Matrix<T>(size, size);

        for (var i = 0; i < size; i++)
        {
            identityMatrix[i, i] = T.One();
        }

        return identityMatrix;
    }

    /// <summary>
    /// Pagalbinis operatorius, leidžiantis lengviau pasiekti matricos elementus.
    /// </summary>
    /// <param name="row">Elemento eilutės pozicija</param>
    /// <param name="column">Elemento stulpelio pozicija</param>
    /// <returns></returns>
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

    /// <summary>
    /// Operatorius, leidžiantis sudėti matricas.
    /// </summary>
    /// <param name="m1">Pirmas operandas</param>
    /// <param name="m2">Antras operandas</param>
    /// <returns>Sumos rezultato matrica</returns>
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

    /// <summary>
    /// Operatorius, leidžiantis sudauginti matricas.
    /// </summary>
    /// <param name="m1">Pirmas operandas</param>
    /// <param name="m2">Antras operandas</param>
    /// <returns>Daugybos rezultato matrica</returns>
    /// <exception cref="ArgumentException"></exception>
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

    /// <summary>
    /// Pagalbinis metodas, leidžiantis konvertuoti matricą į eilutę.
    /// </summary>
    /// <returns>Eilutė, reprezentuojanti matricą</returns>
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

    /// <summary>
    /// Pagalbinis metodas, reikalingas darbui su Dictionary klase.
    /// </summary>
    /// <param name="other">Palyginimo objektas</param>
    /// <returns>true, kai objektai lygūs, kitaip - false</returns>
    public bool Equals(Matrix<T>? other)
    {
        if (other == null)
        {
            return false;
        }

        if (Height != other.Height || Width != other.Width)
        {
            return false;
        }

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (this[y, x] != other[y, x])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Pagalbinis metodas, reikalingas darbui su Dictionary klase.
    /// </summary>
    /// <returns>Matrix objekto hash kodas</returns>
    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 23 + Height.GetHashCode();
        hash = hash * 23 + Width.GetHashCode();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                hash = hash * 23 + this[y, x].GetHashCode();
            }
        }

        return hash;
    }
}