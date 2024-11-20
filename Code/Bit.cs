namespace Code;

using System.Diagnostics.CodeAnalysis;
using Math;

public struct Bit : INumeric<Bit>
{
    public int Value { get; private set; }

    public Bit(int value)
    {
        if (value != 0 && value != 1)
        {
            throw new ArgumentException("Bit value can be either 0 or 1");
        }

        Value = value;
    }

    public Bit Switch()
    {
        return Value == 0 ? new Bit(1) : new Bit(0);
    }

    public static Bit operator +(Bit left, Bit right)
    {
        return new Bit(left.Value ^ right.Value);
    }

    public static Bit operator -(Bit left, Bit right)
    {
        return new Bit(left.Value ^ right.Value);
    }

    public static Bit operator *(Bit left, Bit right)
    {
        return new Bit(left.Value * right.Value);
    }

    public static bool operator ==(Bit left, Bit right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(Bit left, Bit right)
    {
        return left.Value != right.Value;
    }

    public static Bit Zero()
    {
        return new Bit(0);
    }

    public static Bit One()
    {
        return new Bit(1);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == null || obj is not Bit)
        {
            return false;
        }

        return this == (Bit)obj;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}