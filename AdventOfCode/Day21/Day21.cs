using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

internal class Day21 : MapSolution<Day21.GardenCell>
{
    private const int PART_1_STEP_COUNT = 64;
    static readonly string outputFilePath = @"C:\Users\LombardoNick\source\repos\AdventOfCode\AdventOfCode\Day21\Day21Map.txt";
    List<(int, int)> reachablePositions = new List<(int, int)>();
    List<(int, int)> calculationBuffer = new List<(int, int)>();
    (int, int) startPosition;
    string inputFilePath;
    int mapTileSize;
    public Day21()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        inputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");
        mapTileSize = File.ReadAllLines(inputFilePath)[0].Length;
    }


    public override string SolvePart1()
    {
        ReadMap(inputFilePath, GardenCell.GetNewCell);
        FindCellBySymbol('S', out startPosition);
        reachablePositions.Add(startPosition);
        for (int i = 0; i < PART_1_STEP_COUNT; i++)
        {
            PropagateReachablePositions();
        }
        return CountReachablePlots().ToString();
    }

    public override string SolvePart2()
    {
       // GetOffsetsData();
        List<Sequence> sequences = new List<Sequence>();
        foreach (var line in File.ReadLines(outputFilePath))
        {
            var sequence = new Sequence(line.Split(':')[1]);
            sequences.Add(sequence);
        }
        int targetStep = 26501365;
        int iterations = (targetStep+ 1 - 8 * mapTileSize) / mapTileSize;
        for (int i = 0; i < iterations; i++)
        {
                sequences[(targetStep- 1) % mapTileSize].ExtrapolateNextValue();
        }
        // 82
        return sequences[(targetStep - 1) % mapTileSize].ExtrapolateNextValue().ToString();
    }

    private void GetOffsetsData()
    {
        ReadMapPadded(inputFilePath, GardenCell.GetNewCell, 15, 'S');
        File.WriteAllText(outputFilePath, GetMapString());
        FindCellBySymbol('S', out startPosition);
        reachablePositions.Add(startPosition);
        List<Deltas> diffs = new List<Deltas>();
        int iRepeat = mapTileSize;
        List<int>[] offsets = new List<int>[iRepeat];

        int lastDelta = 0;
        int lastReachable = 1;
        for (int i = 0; i < iRepeat * 8; i++)
        {
            PropagateReachablePositions();
            var reachable = CountReachablePlots();
            int delta = reachable - lastReachable;
            int delta2 = lastDelta - delta;
            diffs.Add(new Deltas() { i = i, reachable = reachable, delta = delta, delta2 = delta2 });
            if (i >= 4 * iRepeat)
            {
                int idx = i % iRepeat;
                if (offsets[idx] == null)
                    offsets[idx] = new List<int>();
                offsets[idx].Add(reachable);
            }
            //Console.WriteLine($"{diffs.Last()}");
            lastDelta = delta;
            lastReachable = reachable;
            //File.WriteAllText(mapOutPath, GetMapString());
            // 330
        }  // 461
        WriteOffsets(offsets);
    }

    void WriteOffsets(List<int>[] offsets)
    {
        int i = 0;
        File.WriteAllLines(outputFilePath, offsets.Select(o => $"{i++:d3}: " + string.Join(" ", o)));
    }

    class Deltas
    {
        public int i;
        public int reachable;
        public int delta;
        public int delta2;
        public override string ToString()
        {
            return $"{i}: {reachable} delta: {delta} delta^2: {delta2}";
        }
    }
    int CountReachablePlots()
    {
        return reachablePositions.Count;
    }
    bool flag = false;
    Dictionary<(int, int), (int, int)[]> stepMap = new Dictionary<(int, int), (int, int)[]>();
    void PropagateReachablePositions()
    {
        calculationBuffer.Clear();
        foreach (var startPos in reachablePositions)
        {
            if (!stepMap.ContainsKey(startPos))
            {
                stepMap.Add(startPos,
                directions.Where(dir => IsMoveValid(startPos, dir)).Select(dir =>
                {
                    var delta = GetDelta(dir);

                    if (!flag && !IsOnMap((startPos.Item1 + delta.Item1, startPos.Item2 + delta.Item2)))
                    {
                        flag = true;
                        Console.WriteLine("OOOOOOOOOOO SHIT!");
                    }
                    return GetNeighborPosition(startPos, dir);
                }).ToArray());

                //foreach (Direction dir in directions)
                //{


                //    if (IsMoveValid(startPos, dir))
                //    {
                //        (int, int) reachablePosition = GetNeighborPosition(startPos, dir);
                //        calculationBuffer.Add(reachablePosition);
                //    }
                //}
            }
            calculationBuffer.AddRange(stepMap[startPos]);
        }
        reachablePositions = calculationBuffer.Distinct().ToList();
    }

    protected override char CellToSymbol(GardenCell cell)
    {
        if (reachablePositions.Contains(cell.pos))
            return 'O';
        else
            return base.CellToSymbol(cell);
    }

    internal class GardenCell : Cell
    {

        public GardenCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            isObstacle = symbol == '#';
        }

        public static GardenCell GetNewCell((int, int) pos, char symbol)
        {
            return new GardenCell(pos.Item1, pos.Item2, symbol);
        }
    }
}
