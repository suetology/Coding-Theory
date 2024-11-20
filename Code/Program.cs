using Microsoft.Extensions.Configuration;
using Code.Extensions;
using Code;
using Math;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Drawing.Imaging;

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

var random = new Random();
var channel = new Channel(errorProbability, random);
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

    RemoveAddedBits(ref distortedBits, bitsAdded);

    var distortedText = BitsToText(distortedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message without encoding: ", distortedText);  

    var encodedMessage = coder.Encode(initialMessage);
    var distortedEncodedMessage = channel.DistortMessage(encodedMessage);
    var distortedDecodedMessage = coder.Decode(distortedEncodedMessage);
    var distortedDecodedBits = BitMessageToBits(distortedDecodedMessage);

    RemoveAddedBits(ref distortedDecodedBits, bitsAdded);

    var distortedDecodedText = BitsToText(distortedDecodedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message with encoding: ", distortedDecodedText);
}

void EncodeImage()
{
    OpenFile(message.Content);

    var (width, height, bits) = ReadBitsFromBmpFile(message.Content);

    var bitsAdded = AddMissingBits(bits, code.GeneratorMatrix.Height);

    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, bits);

    var distortedNonEncodedMessage = channel.DistortMessage(initialMessage);
    var distortedBits = BitMessageToBits(distortedNonEncodedMessage);

    RemoveAddedBits(ref distortedBits, bitsAdded);

    var distortedNonEncodedImageFilepath = "./Images/distorted_non-encoded.bmp";

    WriteBitsToBmpFile(distortedNonEncodedImageFilepath, distortedBits, width, height);
    OpenFile(distortedNonEncodedImageFilepath);

    var encodedMessage = coder.Encode(initialMessage);
    var distortedEncodedMessage = channel.DistortMessage(encodedMessage);
    var distortedDecodedMessage = coder.Decode(distortedEncodedMessage);
    var distortedDecodedBits = BitMessageToBits(distortedDecodedMessage);

    RemoveAddedBits(ref distortedDecodedBits, bitsAdded);

    var distortedEncodedImageFilepath = "./Images/distorted_encoded.bmp";

    WriteBitsToBmpFile(distortedEncodedImageFilepath, distortedDecodedBits, width, height);
    OpenFile(distortedEncodedImageFilepath);
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

static byte[] TextToBits(string text)
{
    var bits = new byte[text.Length * 8];
    var i = 0;

    foreach (var character in text)
    {
        var binaryCharacter = Convert.ToString(character, 2).PadLeft(8, '0');

        foreach (var bit in binaryCharacter)
        {
            bits[i] = (byte)(bit == '1' ? 1 : 0);
            i++;
        }
    }

    return bits;
}

static byte[] BitMessageToBits(BitMessage message)
{
    var bits = new byte[message.WordVectors.Count * message.WordVectors[0].Width * 8];
    var k = 0;

    for (var i = 0; i < message.WordVectors.Count; i++)
    {
        for (var j = 0; j < message.WordVectors[i].Width; j++)
        {
            bits[k] = (byte)message.WordVectors[i][0, j].Value;
            k++;
        }
    }

    return bits;
}

static string BitsToText(byte[] bits)
{
    var text = new List<char>();

    for (var i = 0; i < bits.Length; i += 8)
    {
        var byteValue = 0;

        for (var j = i; j < i + 8; j++)
        {
            byteValue += bits[j] * MathUtils.Pow(2, 7 - (j - i));
        }

        text.Add((char)byteValue);
    }

    return new string(text.ToArray());
}

static int AddMissingBits(byte[] bits, int wordLength)
{
    var bitsMissing = wordLength - (bits.Length % wordLength);

    if (bitsMissing == wordLength)
    {
        return 0;
    }

    Array.Resize(ref bits, bits.Length + bitsMissing);

    return bitsMissing;
}

static void RemoveAddedBits(ref byte[] bits, int bitsAdded)
{
    if (bitsAdded <= 0)
    {
        return;
    }

    Array.Resize(ref bits, bits.Length - bitsAdded);
}

static (int, int, byte[]) ReadBitsFromBmpFile(string filepath)
{
    if (!File.Exists(filepath))
    {
        throw new FileNotFoundException(filepath);
    }

    using var bmp = new Bitmap(filepath);

    var width = bmp.Width;
    var height = bmp.Height;

    var bits = new byte[width * height * 3 * 8]; 
    var i = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            var pixel = bmp.GetPixel(x, y);
            var rgbColors = new byte[] { pixel.R, pixel.G, pixel.B };

            foreach (var color in rgbColors)
            {
                var colorValue = color;
                for (int j = 7; j >= 0; j--)
                {
                    var powerOfTwo = MathUtils.Pow(2, j);
                    bits[i] = (byte)(colorValue >= powerOfTwo ? 1 : 0);
                    colorValue -= (byte)(colorValue >= powerOfTwo ? powerOfTwo : 0);
                    i++;
                }
            }
        }
    }

    return (width, height, bits);
}

static void WriteBitsToBmpFile(string filepath, byte[] bits, int width, int height)
{
    var bmp = new Bitmap(width, height);
    var i = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            byte r = 0, g = 0, b = 0;

            for (int j = 0; j < 8; j++)
            {
                var powerOfTwo = MathUtils.Pow(2, 7 - j);
                r += (byte)(bits[i] == 1 ? powerOfTwo : 0);
                i++;
            }

            for (int j = 0; j < 8; j++)
            {
                var powerOfTwo = MathUtils.Pow(2, 7 - j);
                g += (byte)(bits[i] == 1 ? powerOfTwo : 0);
                i++;
            }

            for (int j = 0; j < 8; j++)
            {
                var powerOfTwo = MathUtils.Pow(2, 7 - j);
                b += (byte)(bits[i] == 1 ? powerOfTwo : 0);
                i++;
            }

            bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
        }
    }

    bmp.Save(filepath, ImageFormat.Bmp);
}

static void OpenFile(string filepath)
{
    var absolutePath = Path.GetFullPath(filepath);

    Process.Start("explorer.exe", absolutePath);
}