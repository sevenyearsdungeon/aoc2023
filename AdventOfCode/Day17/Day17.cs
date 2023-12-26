using System.Reflection;
using static Day23;

internal class Day17 : MapSolution<Day17.HeatCell>
{

    PriorityQueue<List<Move>, int> incompletePaths = new PriorityQueue<List<Move>, int>();
    List<List<Move>> completedPaths = new List<List<Move>>();
    Dictionary<Move, int> minCosts = new Dictionary<Move, int>();
    HeatCell start;
    HeatCell end;

    public Day17()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        ReadMap(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt"), HeatCell.Build);
        start = map[0, 0];
        end = map[mapWidth - 1, mapHeight - 1];
    }


    public override string SolvePart1()
    {
        return "";
        minCosts.Clear();
        incompletePaths.Enqueue(new List<Move>() { new Move(start, Direction.Up) },0);
        while (incompletePaths.Count > 0)
        {
            var path = incompletePaths.Dequeue();
            Move lastMove = path.Last();


            foreach (Direction dir in directions)
            {
                int straights = 0;
                if (path.Count >= 3)
                {
                    for (int i = path.Count - 1; i >= path.Count - 3; i--)
                    {
                        if (dir == path[i].dir)
                            straights++;
                    }

                }
                if (straights < 3 && IsMoveValid(lastMove.cell.pos, dir, out HeatCell nextStep) && !path.Any(p => p.cell == nextStep))
                {
                    Move nextMove = new Move(nextStep, dir);
                    int pathCost = path.Sum(move => move.cell.heatLoss);
                    //if (straights == 2)
                    //{
                    //    int costA = int.MaxValue;
                    //    Direction dirA = (Direction)(((int)dir + 1) % 4);
                    //    if (IsMoveValid(nextStep.pos,dirA ))
                    //    {
                    //        costA = GetNeighborCell(nextStep, dirA).heatLoss;
                    //    }
                    //    int costB = int.MaxValue;
                    //    Direction dirB = (Direction)(((int)dir + 3) % 4);
                    //    if (IsMoveValid(nextStep.pos, dirB))
                    //    {
                    //        costB = GetNeighborCell(nextStep, dirB).heatLoss;
                    //    }
                    //    pathCost += Math.Min(costA, costB);
                    //}
                    if (minCosts.ContainsKey(nextMove))
                    {
                        if (pathCost > minCosts[nextMove])
                            continue;
                        else
                            minCosts[nextMove] = pathCost;
                    }
                    else
                        minCosts.Add(nextMove, pathCost);
                    var newPath = new List<Move>(path);

                    newPath.Add(nextMove);
                    if (nextStep != end)
                    {
                        incompletePaths.Enqueue(newPath, pathCost);
                    }
                    else
                    {
                        completedPaths.Add(newPath);
                    }

                }
            }
        }
        int minOccurrences = 0;
        int minCost = int.MaxValue;
        foreach (var path in completedPaths)
        {
            int totalCost = path.Sum(p => p.cell.heatLoss) - 2;
            if (totalCost < minCost)
            {
                minOccurrences = 1;
                minCost = totalCost;
            }
            if (totalCost == minCost)
                minOccurrences++;
        }

        return minCost.ToString();
    }
    // 704 wrong
    // 705 wrong
    // 707 too high

    // 711 to high

    public override string SolvePart2()
    {
        return "";
    }

    int Heuristic(HeatCell a)
    {
        return ManhattanDistance(a, end);
    }

    class Move
    {
        public HeatCell cell;
        public Direction dir;

        public Move(HeatCell cell, Direction dir)
        {
            this.cell = cell;
            this.dir = dir;
        }

        public override bool Equals(object? obj)
        {
            Move move = (Move)obj;
            if (move == null)
                return false;
            return cell == move.cell && dir == move.dir;
        }

        public override int GetHashCode()
        {
            return cell.GetHashCode() + dir.GetHashCode();
        }

        public override string ToString()
        {
            return $"{cell}: {dir}";
        }
    }


    public class HeatCell : Cell
    {
        public readonly int heatLoss;

        public HeatCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            heatLoss = int.Parse(symbol.ToString());
        }
        public static HeatCell Build((int, int) position, char symbol)
        {
            return new HeatCell(position.Item1, position.Item2, symbol);
        }
        public override void Reset()
        {
            base.Reset();
        }
    }

}
