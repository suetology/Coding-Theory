namespace Code;

public class Channel
{
    private readonly float _errorProbability;

    public Channel(float errorProbability)
    {
        _errorProbability = errorProbability;
    }

    public BitMessage DistortMessage(BitMessage message)
    {
        throw new NotImplementedException();
    } 
}