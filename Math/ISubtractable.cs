namespace Math;

public interface ISubtractable<T> where T : ISubtractable<T>
{
    static abstract T operator -(T left, T right);
}