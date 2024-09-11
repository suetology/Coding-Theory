namespace Code.Extensions;

using Code.Configuration;
using Math;
using Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static string GetMessage(this IConfiguration configuration)
    {
        var message = configuration.GetSection("message").Get<string>();

        if (message == null)
        {
            throw new Exception("Message is not specified in configuration");
        }

        if (!message.IsBitString())
        {
            throw new Exception("Message is not a string of bits");
        }

        return message;
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
            var matrix = new Matrix<Bit>(code.Dimension, code.Length);

            for (var y = 0; y < matrixSection.Count; y++)
            {
                for (var x = 0; x < matrixSection[y].Count; x++)
                {
                    matrix[y, x] = new Bit(matrixSection[y][x]);
                }
            }

            code.GeneratorMatrix = matrix;
        }

        return code;
    }
}