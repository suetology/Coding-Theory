using System.Text;
using Code.Extensions;
using Math;

namespace Code;

public class BitMessage
{
    public IList<Matrix<Bit>> WordVectors { get; }

    public BitMessage(IList<Matrix<Bit>> wordVectors)
    {
        WordVectors = wordVectors;
    }

    public BitMessage(int wordLength, string message)
    {
        if (message.Length % wordLength != 0)
        {
            throw new ArgumentException("Invalid length of a message");
        }

        WordVectors = new List<Matrix<Bit>>();

        for (var i = 0; i < message.Length; i += wordLength)
        {
            var word = message.Substring(i, wordLength);
            var vector = word.ToBitVector();

            WordVectors.Add(vector);
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var word in WordVectors)
        {
            builder.Append(word + " ");
        }

        return builder.ToString();
    }
}