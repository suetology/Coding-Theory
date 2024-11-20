namespace Code;

using Code.Extensions;
using Math;

/// <summary>
/// Klasė, skirta kodavimui ir dekodavimui naudojant tiesinio kodo algoritmą.
/// </summary>
public class LinearCoder : ICoder
{
    private readonly Matrix<Bit> _generatorMatrix;

    private readonly Matrix<Bit> _parityCheckMatrix;

    private readonly IDictionary<Matrix<Bit>, int> _syndromeAndLeaderWeightTable;

    /// <summary>
    /// Sukuria LinearCoder objektą iš kodo standartinio pavidalo generuojančios matricos.
    /// </summary>
    /// <param name="generatorMatrix">Standartinio pavidalo generuojanča matrica iš Bit elementų</param>
    public LinearCoder(Matrix<Bit> generatorMatrix)
    {
        _generatorMatrix = generatorMatrix;
        _parityCheckMatrix = _generatorMatrix.CreateParityCheckMatrix();
        _syndromeAndLeaderWeightTable = GetSyndromeAndLeaderWeightTable(_generatorMatrix.Height, _generatorMatrix.Width);
    }

    /// <summary>
    /// Koduoja BitMessage pranešimą.
    /// </summary>
    /// <param name="message">Pradinis pranešimas</param>
    /// <returns>Užkoduotas pranešimas</returns>
    public BitMessage Encode(BitMessage message)
    {
        var encodedWords = new List<Matrix<Bit>>();

        foreach (var word in message.WordVectors)
        {
            encodedWords.Add(EncodeWord(word));
        }

        return new BitMessage(encodedWords);
    }

    /// <summary>
    /// Dekoduoja BitMessage pranešimą.
    /// </summary>
    /// <param name="message">Užkoduotas pranešimas</param>
    /// <returns>Dekoduotas pranešimas</returns>
    public BitMessage Decode(BitMessage message)
    {
        var decodedWords = new List<Matrix<Bit>>();

        foreach (var word in message.WordVectors)
        {
            decodedWords.Add(DecodeWord(word));
        }

        return new BitMessage(decodedWords);
    }

    /// <summary>
    /// Koduoja žodį.
    /// </summary>
    /// <param name="word">Pradinis žodis - Bitų vektorius</param>
    /// <returns>Užkoduotas žodis - Bitų vektorius</returns>
    private Matrix<Bit> EncodeWord(Matrix<Bit> word)
    {
        return word * _generatorMatrix;
    }

    /// <summary>
    /// Dekoduoja žodį.
    /// </summary>
    /// <param name="word">Užkoduotas žodis - Bitų vektorius</param>
    /// <returns>Dekoduotas žodis - Bitų vektorius</returns>
    private Matrix<Bit> DecodeWord(Matrix<Bit> word)
    {
        // Iteruoja pro kiekviena vektoriaus bitą.
        for (var i = 0; i < word.Width; i++)
        {
            // Suskaičiuoja žodžio sindromą.
            var wordSyndrome = CalculateSyndrome(word);

            // Jeigu žodžio sindromas yra sindromų - klasės lyderių svorių lentelėje, ir atitinkamo lyderio svoris yra lygūs nuliui, 
            // skaitom, kad tai yra ištaisytas žodis.
            if (_syndromeAndLeaderWeightTable[wordSyndrome] == 0)
            {
                return word.ExtractSubmatrix(0, 0, 1, _generatorMatrix.Height);
            }
            // Jeigu sindromą atitinkamo lyderio svoris nėra nulis, einam toliau.

            var errorVector = new Matrix<Bit>(word.Height, word.Width);
            errorVector[0, i] = errorVector[0, i].Switch();

            // Sukuria klaidų vektorių su vienetų pozicijoje su indeksų i ir sudeda ji su dabartiniu žodžiu.
            var newWord = word + errorVector;
            var newWordSyndrome = CalculateSyndrome(newWord);

            // Suskaičiuoja žodžio su vieną ištaisytą klaidą sindromą ir palygina jo sindromą atitinkančio 
            // lyderio ir dabartinio žodžio sindromą atitinkančio lyderio svorius. 
            // Jei dabartinio žodžio sindromą atitinkančio lyderio svoris yra didesnis, 
            // skaitom, kad ištaisėm klaidą i pozicijoje ir pakeičiam dabartinį žodį su nauju žodžiu.
            if (_syndromeAndLeaderWeightTable[newWordSyndrome] < _syndromeAndLeaderWeightTable[wordSyndrome])
            {
                word = newWord;
            }
        }

        return word.ExtractSubmatrix(0, 0, 1, _generatorMatrix.Height);
    }

    /// <summary>
    /// Užpildo sindromų - klasės lyderių svorių lentelę.
    /// </summary>
    /// <param name="codeLength">Kodo ilgis, kitaip - kodo generuojančios matricos aukštis</param>
    /// <param name="codeDimension">Kodo dimensija, kitaip - kodo generuojančios matricos plotis</param>
    /// <returns>Sindromų - klasės lyderių svorių lentelę</returns>
    private Dictionary<Matrix<Bit>, int> GetSyndromeAndLeaderWeightTable(int codeLength, int codeDimension)
    {
        // Suskaičiuoja minimalų klasės lyderių kiekį, reikalingą dekodavimui.
        var leadersCount = MathUtils.Pow(2, codeDimension - codeLength);
        var syndromeLeaderWeightTable = new Dictionary<Matrix<Bit>, int>();

        var generateVectorsWithOnes = (Action<Matrix<Bit>, int, int>)null;

        // Pagalbinė rekursyvi lambda funkcija, kuri generuoja visus 
        // įmanomus vektorius ilgio codeDimension, su onesCount vienetų kiekių.
        generateVectorsWithOnes = (Matrix<Bit>vector, int current, int onesCount) =>
        {
            // Jeigu lentelėje jau yra leadersCount eilučių, užbaigiam vektorių generaciją.
            if (syndromeLeaderWeightTable.Keys.Count >= leadersCount)
            {
                return;
            }

            // Base case, kai vektorius jau yra užpildytas.
            if (current == vector.Width)
            {   
                // Suskaičiuoja vektoriaus sindromą ir patikrina, ar yra toks sindromas lentelėje.
                // Jei ne - suskaičiuoja lyderio svorį ir įdeda sindromo - lyderio svorio porą į lentelę.
                var syndrome = CalculateSyndrome(vector);

                if (!syndromeLeaderWeightTable.ContainsKey(syndrome))
                {
                    var weight = CalculateVectorWeight(vector);

                    syndromeLeaderWeightTable.Add(syndrome, weight);
                } 

                return;
            }

            // Recursive case, kai nėra dar panaudoti visi vienetai.
            // Prideda į vektorių vienetą dabartinėje pozicijoje ir rekursyviai kviečia tą pačią funkciją.
            if (onesCount > 0)
            {
                var vectorCopy = new Matrix<Bit>(vector);
                vectorCopy[0, current] = Bit.One();
 
                generateVectorsWithOnes(vectorCopy, current + 1, onesCount - 1);
            }

            // Recursive case, kai vektorius nėra užpildytas iki galo.
            // Prideda į vektorių nulį dabartinėje pozicijoje ir rekursyviai kviečia tą pačią funkciją.
            if (current + onesCount < vector.Width)
            {
                var vectorCopy = new Matrix<Bit>(vector);
                vectorCopy[0, current] = Bit.Zero();

                generateVectorsWithOnes(vectorCopy, current + 1, onesCount);
            }
        };

        // Kviečia vektorius generuojančią funkciją su atitinkamais vienetų kiekiais - nuo 0 iki codeDimention.
        for (var onesCount = 0; onesCount < codeDimension; onesCount++)
        {
            var vector = new Matrix<Bit>(1, codeDimension);

            generateVectorsWithOnes(vector, 0, onesCount);
        }

        return syndromeLeaderWeightTable;
    }

    /// <summary>
    /// Skaičiuoja vektoriaus sindromą.
    /// </summary>
    /// <param name="vector">Vektorius</param>
    /// <returns>Sindromo vektorius</returns>
    private Matrix<Bit> CalculateSyndrome(Matrix<Bit> vector)
    {
        var vectorTranspose = vector.Transpose();

        return _parityCheckMatrix * vectorTranspose;
    }

    /// <summary>
    /// Skaičiuoja vektoriaus svorį - nenulinių bitų kiekį.
    /// </summary>
    /// <param name="vector">Vektorius</param>
    /// <returns>Vektoriaus svoris</returns>
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