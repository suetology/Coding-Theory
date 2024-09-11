namespace Math;

public interface INumeric<T> : IAddable<T>, ISubtractable<T>, IMultipliable<T>
    where T : INumeric<T>
{
    static abstract T Zero();

    static abstract T One();

    static abstract bool operator ==(T left, T right);

    static abstract bool operator !=(T left, T right);
}