namespace Math;

public class Vector<T>
    where T : IAddable<T>, IMultipliable<T>
{
    private readonly T[] _data;

    public int Length => _data.Length;

    public Vector(int length)
    {
        _data = new T[length];
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _data.Length)
            {
                throw new ArgumentOutOfRangeException("Index if out of range");
            }

            return _data[index];
        }
        set
        {
            if (index < 0 || index >= _data.Length)
            {
                throw new ArgumentOutOfRangeException("Index if out of range");
            }

            _data[index] = value;
        }
    }

    public static Vector<T> operator +(Vector<T> v1, Vector<T> v2)
    {
        if (v1.Length != v2.Length)
        {
            throw new ArgumentException("Vectors should have the same length for addition");
        }

        var result = new Vector<T>(v1.Length);

        for (var i = 0; i < v1.Length; i++)
        {
            result[i] = v1[i] + v2[i];
        }

        return result;
    }

    public static 
}