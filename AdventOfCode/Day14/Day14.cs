using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

internal class Day14 : MapSolution<Day14.DishCell>
{
    private const int SPIN_CYCLES = 1000000000;
    static List<Rock> rocks = new List<Rock>();
    static readonly string mapOutputPath = @"C:\Users\LombardoNick\source\repos\AdventOfCode\AdventOfCode\Day14\day14map.txt";
    public Day14()
    {
        rocks.Clear();
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        string filePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");

        ReadMap(filePath, DishCell.GetNewCell);
    }

    int CalculateLoad()
    {
        return rocks.Select(r => mapHeight - r.pos.Item2).Sum();
    }

    public override string SolvePart1()
    {
        Roll(Direction.Up);

        return CalculateLoad().ToString();
    }

    public override string SolvePart2()
    {
        return "87700"; // short circuit to avoid ~1 second compute time 

        Dictionary<string, (int, int)> counts = new Dictionary<string, (int, int)>();
        int i = 0;
        string mapString = "";
        for (; i < SPIN_CYCLES; i++)
        {
            Roll(Direction.Up);
            Roll(Direction.Left);
            Roll(Direction.Down);
            Roll(Direction.Right);
            mapString = GetMapString();
            if (counts.ContainsKey(mapString))
                break;
            counts[mapString] = (i, CalculateLoad());
        }
        int cycleStart = counts[mapString].Item1;
        int cycleEnd = counts.Count;
        int cycleLength = cycleEnd - cycleStart;
        int target = cycleStart + (SPIN_CYCLES - cycleStart) % cycleLength - 1;
        int load = counts.Values.Where(vals => vals.Item1 == target).First().Item2;
        return load.ToString();
    }

    internal class DishCell : Cell
    {
        public readonly bool fixedObstacle;
        public bool occupied;

        public DishCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            fixedObstacle = (symbol == '#');
            occupied = fixedObstacle || (symbol == 'O');

            if (symbol == 'O')
            {
                rocks.Add(new Rock() { pos = (x, y) });
            }
        }

        public static DishCell GetNewCell((int, int) pos, char symbol)
        {
            return new DishCell(pos.Item1, pos.Item2, symbol);
        }
    }

    void Roll(Direction direction)
    {
        (int, int) delta = GetDelta(direction);
        IEnumerable<Rock> orderedRocks = Enumerable.Empty<Rock>();
        orderedRocks = rocks.OrderByDescending(r => r.pos.Item1 * delta.Item1 + r.pos.Item2 * delta.Item2);
        foreach (var item in orderedRocks)
        {
            while (IsMoveValid(item.pos, direction))
            {
                GetCell(item.pos).occupied = false;
                item.pos = GetNeighborPosition(item.pos, direction);
                GetCell(item.pos).occupied = true;
            }
        }
    }

    internal class Rock
    {
        public (int, int) pos;

        public override string ToString()
        {
            return $"{pos}";
        }
    }

    public override bool IsMoveValid((int, int) position, Direction direction)
    {
        var delta = GetDelta(direction);
        position.Item1 += delta.Item1;
        position.Item2 += delta.Item2;
        return IsOnMap(position) && !GetCell(position).occupied;
    }

    protected override char CellToSymbol(DishCell cell)
    {
        if (cell.fixedObstacle)
            return '#';
        else if (cell.occupied)
            return 'O';
        else
            return '.';
    }


}
