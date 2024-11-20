using Microsoft.Extensions.Configuration;
using Code.Extensions;
using Code;
using Math;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("configuration.json", optional: false, reloadOnChange: false)
    .Build();

var errorProbability = configuration.GetErrorProbability();
var code = configuration.GetCodeConfiguration();
var message = configuration.GetMessageConfiguration();

if (code.GeneratorMatrix == null)
{
    code.GeneratorMatrix = MatrixExtensions.CreateGeneratorMatrix(code.Dimension, code.Length);
}

if (!code.GeneratorMatrix.IsStandartFormMatrix())
{
    throw new ArgumentException("Generator matrix must be in standart form");
}

var channel = new Channel(errorProbability);
var coder = new LinearCoder(code.GeneratorMatrix);

switch (message.Type)
{
    case "BitString":
        EncodeBitString();
        break;
    
    case "Text":
        EncodeText();
        break;

    case "Image":
        EncodeImage();
        break;

    default:
        throw new ArgumentException($"Can't procced with message type {message.Type}");
}

void EncodeBitString()
{
    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, message.Content);
    
    Console.WriteLine("{0,-20}{1}", "Initial message: ", initialMessage);

    var encodedMessage = coder.Encode(initialMessage);

    Console.WriteLine("{0,-20}{1}", "Encoded message: ", encodedMessage);

    var distortedMessage = channel.DistortMessage(encodedMessage);

    PrintDistortedMessage(distortedMessage, encodedMessage);

    Console.WriteLine("Enter positions of bits that you want to change, separated by space:");

    var input = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(input))
    {
        var positions = input?
            .Split(' ')
            .Select(int.Parse)
            .ToArray();

        if (positions != null)
        {
            foreach (var position in positions)
            {
                var wordIndex = position / code.GeneratorMatrix.Width;
                var bitIndex = position % code.GeneratorMatrix.Width;

                distortedMessage.WordVectors[wordIndex][0, bitIndex] = distortedMessage.WordVectors[wordIndex][0, bitIndex].Switch();
            }

            PrintDistortedMessage(distortedMessage, encodedMessage);
        }
    }

    var decodedMessage = coder.Decode(distortedMessage);

    Console.WriteLine("{0,-20}{1}", "Decoded message: ", decodedMessage);
}

void EncodeText()
{
    Console.WriteLine("{0,-40}{1}", "Initial message: ", message.Content);

    var bits = TextToBits(message.Content);

    var bitsAdded = AddMissingBits(bits, code.GeneratorMatrix.Height);

    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, bits);

    var distortedNonEncodedMessage = channel.DistortMessage(initialMessage);
    var distortedBits = BitMessageToBits(distortedNonEncodedMessage);

    RemoveAddedBits(distortedBits, bitsAdded);

    var distortedText = BitsToText(distortedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message without encoding: ", distortedText);

    var encodedMessage = coder.Encode(initialMessage);
    var distortedEncodedMessage = channel.DistortMessage(encodedMessage);
    var distortedDecodedMessage = coder.Decode(distortedEncodedMessage);
    var distortedDecodedBits = BitMessageToBits(distortedDecodedMessage);

    RemoveAddedBits(distortedDecodedBits, bitsAdded);

    var distortedDecodedText = BitsToText(distortedDecodedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message with encoding: ", distortedDecodedText);
}

void EncodeImage()
{
}

static void PrintDistortedMessage(BitMessage distortedMessage, BitMessage encodedMessage)
{
    Console.Write("{0,-20}", "Distorted message: ");
    var errorCount = 0;
    var defaultColor = Console.ForegroundColor;

    for (var i = 0; i < encodedMessage.WordVectors.Count; i++)
    {
        for (var j = 0; j < encodedMessage.WordVectors[i].Width; j++)
        {
            if (encodedMessage.WordVectors[i][0, j] == distortedMessage.WordVectors[i][0, j])
            {
                Console.ForegroundColor = defaultColor;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                errorCount++;
            }

            Console.Write(distortedMessage.WordVectors[i][0, j]);
        }
        Console.Write(' ');
    }

    Console.ForegroundColor = defaultColor;
    Console.WriteLine();
    Console.WriteLine("Error count: " + errorCount);
    Console.WriteLine();
}

static List<int> TextToBits(string text)
{
    var bits = new List<int>();

    foreach (var character in text)
    {
        var binaryCharacter = Convert.ToString(character, 2).PadLeft(8, '0');

        foreach (var bit in binaryCharacter)
        {
            bits.Add(bit == '1' ? 1 : 0);
        }
    }

    return bits;
}

static List<int> BitMessageToBits(BitMessage message)
{
    var bits = new List<int>();

    for (var i = 0; i < message.WordVectors.Count; i++)
    {
        for (var j = 0; j < message.WordVectors[i].Width; j++)
        {
            bits.Add(message.WordVectors[i][0, j].Value);
        }
    }

    return bits;
}

static string BitsToText(List<int> bits)
{
    var text = new List<char>();

    for (var i = 0; i < bits.Count; i += 8)
    {
        var byteBits = bits.GetRange(i, 8);

        var byteValue = 0;

        for (var j = 0; j < 8; j++)
        {
            byteValue += byteBits[j] * MathUtils.Pow(2, 7 - j);
        }

        text.Add((char)byteValue);
    }

    return new string(text.ToArray());
}

static int AddMissingBits(List<int> bits, int wordLength)
{
    var bitsMissing = wordLength - bits.Count % wordLength;

    for (var i = 0; i < bitsMissing; i++)
    {
        bits.Add(0);
    }

    return bitsMissing;
}

static void RemoveAddedBits(List<int> bits, int bitsAdded)
{
    bits.RemoveRange(bits.Count - bitsAdded, bitsAdded);
}