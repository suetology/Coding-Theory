namespace Math;

/// <summary>
/// Pagalbinis interfeisas, kuris apibrėžia sandaugos operaciją T tipo objektams.
/// </summary>
public interface IMultipliable<T> where T : IMultipliable<T>
{
    static abstract T operator *(T left, T right);
}