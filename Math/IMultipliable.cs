namespace Math;

public interface IMultipliable<T> where T : IMultipliable<T>
{
    static abstract T operator *(T left, T right);
}