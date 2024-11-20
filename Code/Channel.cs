namespace Code;

public class Channel
{
    private readonly float _errorProbability;

    private readonly Random _random;

    public Channel(float errorProbability, Random random)
    {
        if (errorProbability < 0 || errorProbability > 1)
        {
            throw new ArgumentOutOfRangeException("Error probability should be in range [0; 1]");
        }

        _errorProbability = errorProbability;
        _random = random;
    }

    public BitMessage DistortMessage(BitMessage message)
    {
        var distortedMessage = new BitMessage(message.WordVectors);

        for (var i = 0; i < distortedMessage.WordVectors.Count; i++)
        {
            for (var j = 0; j < distortedMessage.WordVectors[i].Width; j++)
            {
                if (new Random().NextDouble() <= _errorProbability)
                {
                    distortedMessage.WordVectors[i][0, j] = distortedMessage.WordVectors[i][0, j].Switch();
                }
            }
        }

        return distortedMessage;
    } 
}