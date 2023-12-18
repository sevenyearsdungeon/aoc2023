using AdventOfCode;

internal class Day09 : Solution
{
    static List<string> lines;
    static List<Sequence> sequences;
    static Day09()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day09)}\\{nameof(Day09)}.txt")).ToList();
        sequences = lines.Select(line => new Sequence(line)).ToList();
    }

    public override string SolvePart1()
    {
        var result= sequences.Select(s => s.ExtrapolateNextValue()).Sum();
        return result.ToString();
    }

    public override string SolvePart2()
    {
        var result = sequences.Select(s => s.ExtrapolatePreviousValue()).ToList();
        return result.Sum().ToString();
    }

    class Sequence
    {
        private readonly string sequenceString;
        List<int> values;
        List<List<int>> differenceSequences = new List<List<int>>();

        public Sequence(string sequenceString)
        {
            this.sequenceString = sequenceString;
            values = sequenceString.Split(' ').Select(s => int.Parse(s)).ToList();
            differenceSequences.Add(values);
            while (differenceSequences.Last().Any(v => v != 0))
                differenceSequences.Add(GetDifferenceSequence(differenceSequences.Last()));
        }

        List<int> GetDifferenceSequence(List<int> input)
        {
            List<int> difference = new List<int>();
            for (int i = 1; i < input.Count; i++)
            {
                difference.Add(input[i] - input[i - 1]);
            }
            return difference;
        }

        public int ExtrapolateNextValue()
        {
            int sum = 0;
            for (int i = differenceSequences.Count - 2; i >= 0; i--)
            {
                sum += differenceSequences[i].Last();
            }
            return sum;
        }
        public int ExtrapolatePreviousValue()
        {
            int sum = 0;
            for (int i = differenceSequences.Count - 2; i >= 0; i--)
            {
                sum = differenceSequences[i].First()-sum;
            }
            return sum;
        }
    }

}

