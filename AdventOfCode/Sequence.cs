using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Sequence
{
    private readonly string sequenceString;
    List<long> values;
    List<List<long>> differenceSequences = new List<List<long>>();

    public Sequence(string sequenceString)
    {
        this.sequenceString = sequenceString;
        values = sequenceString.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
        differenceSequences.Add(values);
        while (differenceSequences.Last().Any(v => v != 0))
            differenceSequences.Add(GetDifferenceSequence(differenceSequences.Last()));
    }

    List<long> GetDifferenceSequence(List<long> input)
    {
        List<long> difference = new List<long>();
        for (int i = 1; i < input.Count; i++)
        {
            difference.Add(input[i] - input[i - 1]);
        }
        return difference;
    }

    public long ExtrapolateNextValue()
    {
        long sum = 0;
        for (int i = differenceSequences.Count - 2; i >= 0; i--)
        {
            sum += differenceSequences[i].Last();
            differenceSequences[i].Add(sum);
        }
        return sum;
    }

    public long ExtrapolatePreviousValue()
    {
        long sum = 0;
        for (int i = differenceSequences.Count - 2; i >= 0; i--)
        {
            sum = differenceSequences[i].First() - sum;
        }
        return sum;
    }

    public void ExtrapolateTo(long index)
    {

    }
}

