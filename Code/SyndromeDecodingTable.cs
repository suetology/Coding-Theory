using Code.Extensions;
using Math;

namespace Code;

public class SyndromeDecodingTable
{
    private readonly IDictionary<Matrix<Bit>, Matrix<Bit>> _syndromeLeaderTable = new Dictionary<Matrix<Bit>, Matrix<Bit>>();

    public SyndromeDecodingTable(Matrix<Bit> generatorMatrix, Matrix<Bit> parityCheckMatrix)
    {
        var codeWords = generatorMatrix.GetCodeWords();
        var classes = GetCodeClasses(codeWords);
        var leaders = GetClassesLeaders(classes);

        foreach (var leader in leaders)
        {
            var syndrome = CalculateSyndrome(leader);

            if (!_syndromeLeaderTable.ContainsKey(syndrome))
            {
                _syndromeLeaderTable.Add(syndrome, leader);
            }
        }
    }

    private static IList<IList<Matrix<Bit>>> GetCodeClasses(IList<Matrix<Bit>> codeWords)
    {
        throw new NotImplementedException();
    }

    private static IList<Matrix<Bit>> GetClassesLeaders(IList<IList<Matrix<Bit>>> classes)
    {
        throw new NotImplementedException();
    }

    private static Matrix<Bit> CalculateSyndrome(Matrix<Bit> word)
    {
        throw new NotImplementedException();
    }
}