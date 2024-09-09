namespace Math;

public interface IAddable<T> where T : IAddable<T>
{
    static abstract T operator +(T left, T right);
}