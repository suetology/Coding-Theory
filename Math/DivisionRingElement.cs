namespace Math;

public struct DivisionRingElement :
    IAddable<DivisionRingElement>, 
    IMultipliable<DivisionRingElement>,
    ISubtractable<DivisionRingElement>
{
    public int Modulus { get; }

    public int Value { get; }

    public DivisionRingElement(int modulus, int value)
    {        
        if (modulus < 2)
        {
            throw new ArgumentException("Modulus of a division ring element should be >= 2");
        }

        Modulus = modulus;
        Value = value >= 0
            ? value % Modulus
            : (Modulus + (value % Modulus)) % Modulus;
    }

    public static DivisionRingElement operator +(DivisionRingElement e1, DivisionRingElement e2)
    {
        if (e1.Modulus != e2.Modulus)
        {
            throw new ArgumentException("Trying to add elements from different division rings");
        }

        return new DivisionRingElement(e1.Modulus, e1.Value + e2.Value);
    }

    public static DivisionRingElement operator *(DivisionRingElement e1, DivisionRingElement e2)
    {
        if (e1.Modulus != e2.Modulus)
        {
            throw new ArgumentException("Trying to multiply elements from different division rings");
        }

        return new DivisionRingElement(e1.Modulus, e1.Value * e2.Value);
    }

    public static DivisionRingElement operator -(DivisionRingElement e1, DivisionRingElement e2)
    {
        if (e1.Modulus != e2.Modulus)
        {
            throw new ArgumentException("Trying to subtract elements from different division rings");
        }

        return new DivisionRingElement(e1.Modulus, e1.Value - e2.Value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}