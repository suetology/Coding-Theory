namespace Code;

using System.Diagnostics.CodeAnalysis;
using Math;

/// <summary>
/// Struktūra, atitinkanti baigtinį kūną q2.
/// </summary>
public struct Bit : INumeric<Bit>
{
    public int Value { get; private set; }

    /// <summary>
    /// Konstruktorius, sukuriantis Bit objektą iš int tipo reikšmės.
    /// </summary>
    /// <param name="value">Bito reikšmė - turi būti 0 ar 1</param>
    public Bit(int value)
    {
        if (value != 0 && value != 1)
        {
            throw new ArgumentException("Bit value can be either 0 or 1");
        }

        Value = value;
    }

    /// <summary>
    /// Grąžina Bit objektą su priešinga reikšme.
    /// </summary>
    /// <returns>Bit su reikšmė 1, jei pradinio Bit'o reikšmė yra 0, kitaip - Bit su reikšmė 0</returns>
    public Bit Switch()
    {
        return Value == 0 ? new Bit(1) : new Bit(0);
    }

    /// <summary>
    /// Operatorius, leidžiantis sudėti Bit tipo objektus. 
    /// </summary>
    /// <param name="left">Pirmas operandas</param>
    /// <param name="right">Antras operandas</param>
    /// <returns>Bit operandų suma</returns>
    public static Bit operator +(Bit left, Bit right)
    {
        return new Bit(left.Value ^ right.Value);
    }

    /// <summary>
    /// Operatorius, leidžiantis atimti Bit tipo objektus. 
    /// </summary>
    /// <param name="left">Pirmas operandas</param>
    /// <param name="right">Antras operandas</param>
    /// <returns>Bit operandų skirtumas</returns>
    public static Bit operator -(Bit left, Bit right)
    {
        return new Bit(left.Value ^ right.Value);
    }

    /// <summary>
    /// Operatorius, leidžiantis dauginti Bit tipo objektus. 
    /// </summary>
    /// <param name="left">Pirmas operandas</param>
    /// <param name="right">Antras operandas</param>
    /// <returns>Bit operandų sandauga</returns>
    public static Bit operator *(Bit left, Bit right)
    {
        return new Bit(left.Value * right.Value);
    }

    /// <summary>
    /// Operatorius, leidžiantis lyginti Bit tipo objektus.
    /// </summary>
    /// <param name="left">Pirmas operandas</param>
    /// <param name="right">Antras operandas</param>
    /// <returns>true, kai operandų reikšmės lygios, kitaip - false</returns>
    public static bool operator ==(Bit left, Bit right)
    {
        return left.Value == right.Value;
    }

    /// <summary>
    /// Operatorius, leidžiantis lyginti Bit tipo objektus.
    /// </summary>
    /// <param name="left">Pirmas operandas</param>
    /// <param name="right">Antras operandas</param>
    /// <returns>true, kai operandų reikšmės nelygios, kitaip - false</returns>
    public static bool operator !=(Bit left, Bit right)
    {
        return left.Value != right.Value;
    }

    /// <summary>
    /// Pagalbinis metodas, sukūriantis Bit su reikšmė 0.
    /// </summary>
    /// <returns>Bit su reikšmė 0</returns>
    public static Bit Zero()
    {
        return new Bit(0);
    }

    /// <summary>
    /// Pagalbinis metodas, sukūriantis Bit su reikšmė 1.
    /// </summary>
    /// <returns>Bit su reikšmė 1</returns>
    public static Bit One()
    {
        return new Bit(1);
    }

    /// <summary>
    /// Pagalbinis metodas, paverčiantis Bit objektą į eilutę.
    /// </summary>
    /// <returns>Eilutė iš Bit objekto reišmės</returns>
    public override string ToString()
    {
        return Value.ToString();
    }

    /// <summary>
    /// Pagalbinis metodas, reikalingas darbui su Dictionary klase.
    /// </summary>
    /// <param name="obj">Palyginimo objektas</param>
    /// <returns>true, kai objektai lygūs, kitaip - false</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null || obj is not Bit)
        {
            return false;
        }

        return this == (Bit)obj;
    }

    /// <summary>
    /// Pagalbinis metodas, reikalingas darbui su Dictionary klase.
    /// </summary>
    /// <returns>Bit objekto hash kodas</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}