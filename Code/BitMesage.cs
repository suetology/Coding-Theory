using System.Text;
using Code.Extensions;
using Math;

namespace Code;

public class BitMessage
{
    public IList<Matrix<Bit>> WordVectors { get; }

    public BitMessage(IList<Matrix<Bit>> wordVectors)
    {
        WordVectors = new List<Matrix<Bit>>(wordVectors.Count);

        for (var i = 0; i < wordVectors.Count; i++)
        {
            WordVectors.Add(new Matrix<Bit>(wordVectors[i]));
        }
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

    public BitMessage(int wordLength, byte[] bits)
    {
        if (bits.Length % wordLength != 0)
        {
            throw new ArgumentException("Invalid length of a message");
        }

        WordVectors = new List<Matrix<Bit>>();

        var wordCount = bits.Length / wordLength;

        for (var i = 0; i < wordCount; i++)
        {
            WordVectors.Add(new Matrix<Bit>(1, wordLength));

            for (var j = 0; j < wordLength; j++)
            {
                WordVectors[i][0, j] = new Bit(bits[i * wordLength + j]);
            }
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        foreach (var word in WordVectors)
        {
            for (var i = 0; i < word.Width; i++)
            {
                builder.Append(word[0, i]);
            }
            builder.Append(' ');
        }

        return builder.ToString();
    }
}