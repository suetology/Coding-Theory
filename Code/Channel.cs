namespace Code;

/// <summary>
/// Klasė, simuliuojanti kanalą
/// </summary>
public class Channel
{
    private readonly float _errorProbability;

    private readonly Random _random;

    /// <summary>
    /// Sukuria Channel objectą.
    /// </summary>
    /// <param name="errorProbability">Klaidos tikimybė</param>
    /// <param name="random">Atsitiktinių skaičių generatorius</param>
    public Channel(float errorProbability, Random random)
    {
        if (errorProbability < 0 || errorProbability > 1)
        {
            throw new ArgumentOutOfRangeException("Error probability should be in range [0; 1]");
        }

        _errorProbability = errorProbability;
        _random = random;
    }

    /// <summary>
    /// Sukuria naują BitMessage objektą, kuriame kiekvienas bitas gali būti pakeistas su klados tikimybe.
    /// </summary>
    /// <param name="message">Pradinis pranešimas</param>
    /// <returns>Pakeistas pranešimas</returns>
    public BitMessage DistortMessage(BitMessage message)
    {
        var distortedMessage = new BitMessage(message.WordVectors);

        for (var i = 0; i < distortedMessage.WordVectors.Count; i++)
        {
            for (var j = 0; j < distortedMessage.WordVectors[i].Width; j++)
            {
                if (_random.NextDouble() < _errorProbability)
                {
                    distortedMessage.WordVectors[i][0, j] = distortedMessage.WordVectors[i][0, j].Switch();
                }
            }
        }

        return distortedMessage;
    } 
}