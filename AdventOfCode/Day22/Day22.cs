using AdventOfCode;
using System.Reflection;

internal class Day22 : Solution
{

    static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    static string inputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");
    static string outputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}output.txt");

    static List<Brick> bricks;
    static bool[,,] volume;

    static Day22()
    {
        var lines = File.ReadAllLines(inputFilePath).ToList();
        bricks = lines.Select(s => new Brick(s)).ToList();
        var maxX = bricks.Max(brick => Math.Max(brick.start.x, brick.end.x));
        var maxY = bricks.Max(brick => Math.Max(brick.start.y, brick.end.y));
        var maxZ = bricks.Max(brick => Math.Max(brick.start.z, brick.end.z));
        volume = new bool[maxX,maxY,maxZ];
    }

    public override string SolvePart1()
    {
        return "";
    }

    public override string SolvePart2()
    {
        return "";
    }

    class Brick
    {
        public Vector3 start, end;
        public Brick(string brickString)
        {
            var startInts = brickString.GetSequenceBefore("~", ",").Select(s => int.Parse(s)).ToArray();
            var endInts = brickString.GetSequenceAfter("~", ",").Select(s => int.Parse(s)).ToArray();
            start = new Vector3(startInts[0], startInts[1], startInts[2]);
            end = new Vector3(endInts[0], endInts[1], endInts[2]);
        }

    }

    class Vector3
    {
        public int x, y, z;

        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

}

