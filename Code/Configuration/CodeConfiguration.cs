namespace Code.Configuration;

using Math;

/// <summary>
/// Klasė, kuri atitinka kodo nustatymus, nuskaitomus iš 'configuration.json' failo.
/// </summary>
public class CodeConfiguration
{
    public int Length { get; set; }

    public int Dimension { get; set; }

    public Matrix<Bit>? GeneratorMatrix { get; set; }
}