using Math;

namespace Code.Extensions;

/// <summary>
/// Klasė, skirta praplėsti string klasę pagalbiniais metodais.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Paverčia eilutę, sudarytą iš nulių ir vienetų į bitų vektorių. 
    /// </summary>
    /// <returns>Bitų vektorius - matricą iš bitų, kurios aukštis yra 1</returns>
    public static Matrix<Bit> ToBitVector(this string str)
    {
        if (!str.IsBitString())
        {
            throw new ArgumentException("Can't convert non-bit string to bit vector");
        }

        var vector = new Matrix<Bit>(1, str.Length);

        for (var i = 0; i < str.Length; i++)
        {
            vector[0, i] = new Bit((int)char.GetNumericValue(str[i])); 
        }

        return vector;
    }

    /// <summary>
    /// Patikrina, ar eilutė yra sudaryta tik iš nulių ir vienetų.
    /// </summary>
    /// <returns>true, kai eilutė yra sudaryta tik iš nulių ir vienetų, kitais atvejais - false</returns>
    public static bool IsBitString(this string str)
    {
        return str.All(c => c == '0' || c == '1');
    }
}