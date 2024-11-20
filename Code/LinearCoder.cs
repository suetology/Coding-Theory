namespace Code;

using Code.Extensions;
using Math;

public class LinearCoder : ICoder
{
    private readonly Matrix<Bit> _generatorMatrix;

    private readonly Matrix<Bit> _parityCheckMatrix;

    private readonly IDictionary<Matrix<Bit>, int> _syndromeAndLeaderWeightTable;

    public LinearCoder(Matrix<Bit> generatorMatrix)
    {
        _generatorMatrix = generatorMatrix;
        _parityCheckMatrix = _generatorMatrix.CreateParityCheckMatrix();
        _syndromeAndLeaderWeightTable = GetSyndromeAndLeaderWeightTable(_generatorMatrix.Height, _generatorMatrix.Width);
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
        for (var i = 0; i < word.Width; i++)
        {
            var wordSyndrome = CalculateSyndrome(word);

            if (_syndromeAndLeaderWeightTable[wordSyndrome] == 0)
            {
                return word.ExtractSubmatrix(0, 0, 1, _generatorMatrix.Height);
            }

            var errorVector = new Matrix<Bit>(word.Height, word.Width);
            errorVector[0, i] = errorVector[0, i].Switch();

            var newWord = word + errorVector;
            var newWordSyndrome = CalculateSyndrome(newWord);

            if (_syndromeAndLeaderWeightTable[newWordSyndrome] < _syndromeAndLeaderWeightTable[wordSyndrome])
            {
                word = newWord;
            }
        }

        return word.ExtractSubmatrix(0, 0, 1, _generatorMatrix.Height);
    }

    private Dictionary<Matrix<Bit>, int> GetSyndromeAndLeaderWeightTable(int codeLength, int codeDimension)
    {
        var leadersCount = MathUtils.Pow(2, codeDimension - codeLength);
        var syndromeLeaderWeightTable = new Dictionary<Matrix<Bit>, int>();

        var generateVectorsWithOnes = (Action<Matrix<Bit>, int, int>)null;

        generateVectorsWithOnes = (Matrix<Bit>vector, int current, int onesCount) =>
        {
            if (syndromeLeaderWeightTable.Keys.Count >= leadersCount)
            {
                return;
            }

            if (current == vector.Width)
            {
                var syndrome = CalculateSyndrome(vector);

                if (!syndromeLeaderWeightTable.ContainsKey(syndrome))
                {
                    var weight = CalculateVectorWeight(vector);

                    syndromeLeaderWeightTable.Add(syndrome, weight);
                } 

                return;
            }

            if (onesCount > 0)
            {
                var vectorCopy = new Matrix<Bit>(vector);
                vectorCopy[0, current] = Bit.One();
 
                generateVectorsWithOnes(vectorCopy, current + 1, onesCount - 1);
            }

            if (current + onesCount < vector.Width)
            {
                var vectorCopy = new Matrix<Bit>(vector);
                vectorCopy[0, current] = Bit.Zero();

                generateVectorsWithOnes(vectorCopy, current + 1, onesCount);
            }
        };

        for (var onesCount = 0; onesCount < codeDimension; onesCount++)
        {
            var vector = new Matrix<Bit>(1, codeDimension);

            generateVectorsWithOnes(vector, 0, onesCount);
        }

        return syndromeLeaderWeightTable;
    }

    private Matrix<Bit> CalculateSyndrome(Matrix<Bit> vector)
    {
        var vectorTranspose = vector.Transpose();

        return _parityCheckMatrix * vectorTranspose;
    }

    private static int CalculateVectorWeight(Matrix<Bit> vector)
    {
        var weight = 0;

        for (var i = 0; i < vector.Width; i++)
        {
            if (vector[0, i] == Bit.One())
            {
                weight++;
            }
        }

        return weight;
    }
}