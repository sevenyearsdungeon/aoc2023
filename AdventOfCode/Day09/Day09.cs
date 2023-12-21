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


}

