namespace Math;

/// <summary>
/// Pagalbinis interfeisas, kuris apibrėžia sudėties operaciją T tipo objektams.
/// </summary>
public interface IAddable<T> where T : IAddable<T>
{
    static abstract T operator +(T left, T right);
}