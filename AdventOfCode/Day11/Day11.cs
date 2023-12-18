using AdventOfCode;
using System.Reflection;

internal class Day11 : Solution
{
    static List<string> lines;
    static Day11()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt")).ToList();
    }
    public override string SolvePart1()
    {
        throw new NotImplementedException();
    }

    public override string SolvePart2()
    {
        throw new NotImplementedException();
    }
}

