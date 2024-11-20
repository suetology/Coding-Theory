namespace Code.Extensions;

using Code.Configuration;
using Math;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Klasė, skirta nuskaityti programos įvesti iš 'configuration.json' failo.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gauna klaidos tikimybė iš 'configuration.json' failo.
    /// </summary>
    /// <returns>Klaidos tikimybė</returns>
    public static float GetErrorProbability(this IConfiguration configuration)
    {
        return configuration.GetSection("p").Get<float>();
    }

    /// <summary>
    /// Gauna pranešimo nustatymus iš 'configuration.json' failo.
    /// </summary>
    /// <returns>Pranešimo nustatymai</returns>
    public static MessageConfiguration GetMessageConfiguration(this IConfiguration configuration)
    {
        var messageSection = configuration.GetSection("message");

        var message = new MessageConfiguration();

        message.Type = messageSection.GetSection("type").Get<string>();
        message.Content = messageSection.GetSection("content").Get<string>();

        return message;
    }

    /// <summary>
    /// Gauna kodo nustatymus iš 'configuration.json' failo.
    /// </summary>
    /// <returns></returns>
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