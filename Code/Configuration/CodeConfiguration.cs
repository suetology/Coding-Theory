namespace Code.Configuration;

using Math;

public class CodeConfiguration
{
    public int Length { get; set; }

    public int Dimension { get; set; }

    public Matrix<Bit>? GeneratorMatrix { get; set; }
}