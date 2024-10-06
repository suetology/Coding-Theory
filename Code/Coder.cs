namespace Code;

using Code.Extensions;
using Math;

public class Coder
{
    private readonly Matrix<Bit> _generatorMatrix;

    private readonly Matrix<Bit> _parityCheckMatrix;

    public Coder(Matrix<Bit> generatorMatrix)
    {
        _generatorMatrix = generatorMatrix;
        _parityCheckMatrix = _generatorMatrix.CreateParityCheckMatrix();
    }

    public BitMessage Encode(BitMessage message)
    {
        var encodedWords = new List<Matrix<Bit>>();

        foreach (var word in message.WordVectors)
        {
            encodedWords.Add(EncodeWord(word));
        }

        return new BitMessage(encodedWords);
    }

    public BitMessage Decode(BitMessage message)
    {
        var decodedWords = new List<Matrix<Bit>>();

        foreach (var word in message.WordVectors)
        {
            decodedWords.Add(DecodeWord(word));
        }

        return new BitMessage(decodedWords);
    }

    private Matrix<Bit> EncodeWord(Matrix<Bit> word)
    {
        return word * _generatorMatrix;
    }

    private Matrix<Bit> DecodeWord(Matrix<Bit> word)
    {
        throw new NotImplementedException();
    }
}