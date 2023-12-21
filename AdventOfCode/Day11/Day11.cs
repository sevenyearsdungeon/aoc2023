using AdventOfCode;
using System.Reflection;

internal class Day11 : Solution
{
    static List<string> lines;
    static WeightedCell[,] map;
    static List<WeightedCell> galaxies = new List<WeightedCell>();

    static int startX, startY, mapWidth, mapHeight;
    static Day11()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt")).ToList(); mapWidth = lines[0].Length;
        mapHeight = lines.Count;
        map = new WeightedCell[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                char symbol = lines[mapHeight - y - 1][x];
                WeightedCell newCell = new WeightedCell(x, y, symbol);
                map[x, y] = newCell;
                if (symbol == '#')
                {
                    galaxies.Add(newCell);
                }
            }




    }

    public void SetExpansion(int expansion)
    {

        foreach (int x in Enumerable.Range(0, mapWidth).Except(galaxies.Select(g => g.x)))
            for (int y = 0; y < mapHeight; y++)
                map[x, y].weight = expansion;

        foreach (int y in Enumerable.Range(0, mapHeight).Except(galaxies.Select(g => g.y)))
            for (int x = 0; x < mapWidth; x++)
                map[x, y].weight = expansion;
    }

    public override string SolvePart1()
    {
        SetExpansion(2);
        int distSum = 0;
        for (int i = 0; i < galaxies.Count; i++)
            for (int j = i + 1; j < galaxies.Count; j++)
            {
                if (i == j) continue;
                distSum += L2Distance(galaxies[i], galaxies[j]);
            }
        return distSum.ToString();
    }

    public override string SolvePart2()
    {
        SetExpansion(1000000);
        long distSum = 0;
        for (int i = 0; i < galaxies.Count; i++)
            for (int j = i + 1; j < galaxies.Count; j++)
            {
                if (i == j) continue;

                distSum += L2Distance(galaxies[i], galaxies[j]);
            }
        return distSum.ToString();
    }


    class WeightedCell : Cell
    {
        public int weight = 1;

        public WeightedCell(int x, int y, char symbol) : base(x, y, symbol)
        {
        }
    }

    static int L2Distance(Cell cell1, Cell cell2)
    {
        int sum = 0;
        int minX = Math.Min(cell1.x, cell2.x);
        int maxX = Math.Max(cell1.x, cell2.x);
        int rangeX = Math.Abs(cell1.x - cell2.x);
        int minY = Math.Min(cell1.y, cell2.y);
        int rangeY = Math.Abs(cell1.y - cell2.y);

        for (int x = minX; x < maxX; x++)
        {
            sum += map[x, minY].weight;
        }

        for (int y = minY; y < minY + rangeY; y++)
        {
            sum += map[maxX, y].weight;
        }
        return sum;
    }
}

