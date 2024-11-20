namespace Math;

/// <summary>
/// Pagalbinė klasė, apibrėžianti pagalbinius matematinius metodus.
/// </summary>
public static class MathUtils
{
    public static int Pow(int number, int power)
    {
        if (power < 0)
        {
            throw new ArgumentOutOfRangeException("Power should be non-negative");
        }

        if (power == 0)
        {
            return 1;
        }

        return number * Pow(number, power - 1);
    }
}