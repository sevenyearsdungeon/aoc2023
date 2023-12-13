

using AdventOfCode;

class Day1 : Solution
{
    static readonly string[] lines;
    static readonly string[] numberStrings = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    static readonly string[] alphaNumberStrings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    static Day1()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Day01\\day1.txt"));
    }


    public override string SolvePart1()
    {
        int sum = 0;
        foreach (var line in lines)
        {
            sum += getSum(line, numberStrings);
        }
        return sum.ToString();
    }

    public override string SolvePart2()
    {
        int sum = 0;
        foreach (var line in lines)
        {
            sum += getSum(line, alphaNumberStrings);
        }
        return sum.ToString();
    }



    private int getSum(string line, string[] checkStrings)
    {
        int minIndex = int.MaxValue;
        int maxIndex = int.MinValue;
        int minValue = 0;
        int maxValue = 0;
        for (int i = 0; i < checkStrings.Length; i++)
        {
            string numberString = checkStrings[i];
            int value = i % 10;
            int idx0 = line.IndexOf(numberString);
            if (idx0 < 0)
                continue;
            if (idx0 < minIndex)
            {
                minIndex = idx0;
                minValue = value;
            }
            int idx1 = line.LastIndexOf(numberString);
            if (idx1 > maxIndex)
            {
                maxIndex = idx1;
                maxValue = value;
            }
        }
        return minValue * 10 + maxValue;
    }
}