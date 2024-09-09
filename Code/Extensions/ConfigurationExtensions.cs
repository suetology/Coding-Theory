namespace Code.Extensions;

using Math;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static int GetDivisionRingOrder(this IConfiguration configuration)
    {
        return configuration.GetSection("q").Get<int>();
    }

    public static float GetErrorProbability(this IConfiguration configuration)
    {
        return configuration.GetSection("p").Get<float>();
    }

    public static CodeConfiguration GetCodeConfiguration(this IConfiguration configuration)
    {
        var codeSection = configuration.GetSection("code");

        var code = new CodeConfiguration();

        code.Length = codeSection.GetSection("n").Get<int>();
        code.Dimension = codeSection.GetSection("k").Get<int>();

        var matrixSection = codeSection.GetSection("G").Get<IList<IList<int>>>();

        if (matrixSection != null)
        {
            var q = configuration.GetDivisionRingOrder();

            var matrix = new Matrix<DivisionRingElement>(code.Dimension, code.Length);

            for (var y = 0; y < matrixSection.Count; y++)
            {
                for (var x = 0; x < matrixSection[y].Count; x++)
                {
                    matrix[y, x] = new DivisionRingElement(q, matrixSection[y][x]);
                }
            }

            code.Matrix = matrix;
        }

        return code;
    }
}