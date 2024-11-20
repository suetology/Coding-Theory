using Microsoft.Extensions.Configuration;
using Code.Extensions;
using Code;
using Math;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Imaging;

// Inicializuoja configuration objektą, kuris bus naudojamas programos įvesčiai nuskaityt iš 'configuration.json' failo.
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("configuration.json", optional: false, reloadOnChange: false)
    .Build();

// Nuskaito iš 'configuration.json' failo klaidų tikimybę, kodo nustatymus ir pranešimą.
var errorProbability = configuration.GetErrorProbability();
var code = configuration.GetCodeConfiguration();
var message = configuration.GetMessageConfiguration();

// Jeigu kodo generuojačia matrica nebuvo nurodyta 'configuration.json' faile, sukuria atsitiktinę
// standartinio pavidalo generuojančia matricą.
if (code.GeneratorMatrix == null)
{
    code.GeneratorMatrix = MatrixExtensions.CreateGeneratorMatrix(code.Dimension, code.Length);
}

// Programa palaiko tik darba su standartinio pavidalo generuojančia matrica.
if (!code.GeneratorMatrix.IsStandartFormMatrix())
{
    throw new ArgumentException("Generator matrix must be in standart form");
}

// Inicializuoja atsitiktinių skaičių generatorių, kanalą ir kodavimo/dekodavimo modulį.
var random = new Random();
var channel = new Channel(errorProbability, random);
var coder = new LinearCoder(code.GeneratorMatrix);

// Programa palaiko tris scenarijus - darbą su pranešimu iš bitų eilutės, darbą su tekstu, darbą su paveiksleliu .bmp formato. 
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

// Metodas, dirbantis su pranešimu iš bitų eilutės.
void EncodeBitString()
{
    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, message.Content);

    // Parodo pradinį pranešimą.   
    Console.WriteLine("{0,-20}{1}", "Initial message: ", initialMessage);

    // Užkoduoja pranešimą ir parodo jį.
    var encodedMessage = coder.Encode(initialMessage);

    Console.WriteLine("{0,-20}{1}", "Encoded message: ", encodedMessage);

    // Siunčia pranešimą kanalų ir parodo pakeistą pranešimą.
    var distortedMessage = channel.DistortMessage(encodedMessage);

    PrintDistortedMessage(distortedMessage, encodedMessage);

    // Jeigu reikia, vartotojas gali pakeisti pageidautinus bitus savarankiškai.
    Console.WriteLine("Enter positions of bits that you want to change, separated by space:");

    var input = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(input))
    {
        var positions = input?
            .Split(' ')
            .Select(int.Parse)
            .ToArray();

        // Pakeičia bitus pozicijose, kurias pasirinko vartotojas ir dar kartą parodo pakeistą pranešimą.
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

    // Dekoduoja pranešimą ir jį parodo.
    var decodedMessage = coder.Decode(distortedMessage);

    Console.WriteLine("{0,-20}{1}", "Decoded message: ", decodedMessage);
}

// Metodas, dirbantis su pranešimu iš tekstinės eilutės.
void EncodeText()
{
    // Parodo pradinį pranešimą.
    Console.WriteLine("{0,-40}{1}", "Initial message: ", message.Content);

    // Konvertuoją eilutę į bitų masyvą.
    var bits = TextToBits(message.Content);

    // Jeigų reikia, prideda kelis bitus, kad bendras bitų skaičius būtų kodo ilgio kartotinis.
    var bitsAdded = AddMissingBits(bits, code.GeneratorMatrix.Height);

    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, bits);

    // Siunčia NEužkoduotą pranešimą kanalų, ir konvertuoja gautą iš kanalo pranešimą į bitų masyvą.
    var distortedNonEncodedMessage = channel.DistortMessage(initialMessage);
    var distortedBits = BitMessageToBits(distortedNonEncodedMessage);

    // Panaikina pridėtus bitus.
    RemoveAddedBits(ref distortedBits, bitsAdded);

    // Atstato pranešimą, konvertuojant bitų masyvą į tekstinę eilutę ir parodo ją.
    var distortedText = BitsToText(distortedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message without encoding: ", distortedText);  

    // Užkoduoja pranešimą, siunčia jį kanalų, dekoduoja ir konvertuoja į bitų masyvą.
    var encodedMessage = coder.Encode(initialMessage);
    var distortedEncodedMessage = channel.DistortMessage(encodedMessage);
    var distortedDecodedMessage = coder.Decode(distortedEncodedMessage);
    var distortedDecodedBits = BitMessageToBits(distortedDecodedMessage);

    // Panaikina pridėtus bitus.
    RemoveAddedBits(ref distortedDecodedBits, bitsAdded);

    // Atstato pranešimą, konvertuojant bitų masyvą į tekstinę eilutę ir parodo ją.
    var distortedDecodedText = BitsToText(distortedDecodedBits);

    Console.WriteLine("{0,-40}{1}", "Distorted message with encoding: ", distortedDecodedText);
}

// Metodas, dirbantis su .bmp formato paveiksleliais.
void EncodeImage()
{
    // Parodo pradinį paveikslelį.
    OpenFile(message.Content);

    // Nuskaito paveikslelio piksleių bitus. 
    var (width, height, bits) = ReadBitsFromBmpFile(message.Content);

    // Jeigų reikia, prideda kelis bitus, kad bendras bitų skaičius būtų kodo ilgio kartotinis.
    var bitsAdded = AddMissingBits(bits, code.GeneratorMatrix.Height);

    var initialMessage = new BitMessage(code.GeneratorMatrix.Height, bits);

    // Siunčia NEužkoduotą pranešimą kanalų, ir konvertuoja gautą iš kanalo pranešimą į bitų masyvą.
    var distortedNonEncodedMessage = channel.DistortMessage(initialMessage);
    var distortedBits = BitMessageToBits(distortedNonEncodedMessage);

    // Panaikina pridėtus bitus.
    RemoveAddedBits(ref distortedBits, bitsAdded);

    var distortedNonEncodedImageFilepath = "./Images/distorted_non-encoded.bmp";

    // Įrašo gautus bitus į naujai sukurtą bmp failą ir parodo gautą paveikslelį.
    WriteBitsToBmpFile(distortedNonEncodedImageFilepath, distortedBits, width, height);
    OpenFile(distortedNonEncodedImageFilepath);

    // Užkoduoja pranešimą, siunčia jį kanalų, dekoduoja ir konvertuoja į bitų masyvą.
    var encodedMessage = coder.Encode(initialMessage);
    var distortedEncodedMessage = channel.DistortMessage(encodedMessage);
    var distortedDecodedMessage = coder.Decode(distortedEncodedMessage);
    var distortedDecodedBits = BitMessageToBits(distortedDecodedMessage);

    // Panaikina pridėtus bitus.
    RemoveAddedBits(ref distortedDecodedBits, bitsAdded);

    var distortedEncodedImageFilepath = "./Images/distorted_encoded.bmp";

    // Įrašo gautus bitus į naujai sukurtą bmp failą ir parodo gautą paveikslelį.
    WriteBitsToBmpFile(distortedEncodedImageFilepath, distortedDecodedBits, width, height);
    OpenFile(distortedEncodedImageFilepath);
}

// Pagalbinis metodas, skirtas gražesniai reprezentacijai gauto iš kanalo pranešimo.
// Parodo bitus, kurių pozicijose įvyko klaidos, raudona spalva.
// Taip pat parodo įvykusių klaidų kiekį.
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

// Pagalbinis metodas, konvertuojantis tekstinę eilutę į bitų masyvą.
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

// Pagalbinis metodas, atkuriantis bitų masyvą iš BitMessage objekto.
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

// Pagalbinis metodas, atkuriantis tekstinę eilutę iš bitų masyvo.
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

// Pagalbinis metodas, pridedantis bitus, kad bendras bitų skaičius būtų kodo ilgio kartotinis.
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

// Pagalbinis metodas, atimantis pridėtus bitus.
static void RemoveAddedBits(ref byte[] bits, int bitsAdded)
{
    if (bitsAdded <= 0)
    {
        return;
    }

    Array.Resize(ref bits, bits.Length - bitsAdded);
}

// Pagalbinis metodas, kuris nuskaito pikselių informaciją iš bmp failo ir 
// paverčia tą informaciją į bitų masyvą (papildomai grąžina paveikslelio plotį ir aukšti).
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

// Pagalbinis metodas, kuris atstato bmp failą iš bitų masyvo.
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

// Pagalbinis metodas, kuris atidaro failą.
static void OpenFile(string filepath)
{
    var absolutePath = Path.GetFullPath(filepath);

    Process.Start("explorer.exe", absolutePath);
}