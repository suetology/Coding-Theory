namespace Math;

/// <summary>
/// Pagalbinis interfeisas, kuris apibrėžia atimties operaciją T tipo objektams.
/// </summary>
public interface ISubtractable<T> where T : ISubtractable<T>
{
    static abstract T operator -(T left, T right);
}