using Math;

namespace Code.Extensions;

public static class StringExtensions
{
    public static bool IsBitString(this string str)
    {
        return str.All(c => c == '0' || c == '1');
    }

    public static Matrix<Bit> ToBitVector(this string str)
    {
        var vector = new Matrix<Bit>(1, str.Length);

        for (var i = 0; i < str.Length; i++)
        {
            vector[0, i] = new Bit((int)char.GetNumericValue(str[i])); 
        }

        return vector;
    }
}