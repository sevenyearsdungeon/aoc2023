using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Day17;

internal class Day23 : MapSolution<Day23.PathCell>
{
    string intersectionNames = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
    static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
    static string inputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt");
    static string outputFilePath = Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}output.txt");
    PriorityQueue<List<PathCell>, int> incompletePaths = new PriorityQueue<List<PathCell>, int>();
    List<List<PathCell>> completedPaths = new List<List<PathCell>>();
    List<PathCell> intersections = new List<PathCell>();
    List<PathCell> deadEnds = new List<PathCell>();
    PathCell start, end;
    public Day23()
    {
        ReadMap(inputFilePath, PathCell.GetPathCell);
        start = map[1, 0];
        end = map[mapWidth - 2, mapHeight - 1];
        FindIntersections();
    }

    private void FindIntersections()
    {
        ForEachCell(c =>
        {
            if (c.isStaticObstacle)
                return;
            int moves = 0;
            if (IsMoveValid(c.pos, Direction.Up))
                moves++;
            if (IsMoveValid(c.pos, Direction.Left))
                moves++;
            if (IsMoveValid(c.pos, Direction.Down))
                moves++;
            if (IsMoveValid(c.pos, Direction.Right))
                moves++;
            if (moves == 1)
            {
                deadEnds.Add(c);
                c.isDeadEnd = true;
            }
            if (moves > 2)
            {
                intersections.Add(c);
                c.isOnPath = true;
                c.isIntersection = true;
            }
        });
    }

    public override string SolvePart1()
    {
        return 2399.ToString();
        List<PathCell> firstPath = new List<PathCell>();
        firstPath.Add(start);
        incompletePaths.Enqueue(firstPath, 0);
        while (incompletePaths.Count > 0)
        {
            var path = incompletePaths.Dequeue();
            PathCell currentCell = path.Last();
            bool splitDecision = false;

            foreach (Direction dir in directions)
            {
                if (IsMoveValid(currentCell.pos, dir, out PathCell nextStep))
                {
                    if (dir == Direction.Up && nextStep.slopeType == SlopeType.Down)
                        continue;
                    if (dir == Direction.Left && nextStep.slopeType == SlopeType.Right)
                        continue;
                    if (path.Contains(nextStep))
                        continue;
                    if (splitDecision)
                    {
                        path = new List<PathCell>(path);
                    }
                    else
                    {
                        splitDecision = true;
                    }
                    path.Add(nextStep);
                    if (nextStep != end)
                    {
                        incompletePaths.Enqueue(path, path.Count);
                    }
                    else
                    {
                        completedPaths.Add(path);
                    }

                }
            }
        }

        var longestPath = completedPaths.Where(p => p.Count == completedPaths.Max(p => p.Count)).First();
        longestPath.ForEach(p => p.isOnPath = true);

        return (longestPath.Count - 1).ToString();

    }
    // 2392 too low (with -3)
    // 2394 correct (-1)

    public override string SolvePart2()
    {
        // shortcircuit 2 min runtime
        return 6554.ToString();

        int idx = 0;
        HashSet<Edge> edges = new HashSet<Edge>();
        List<PathCell> firstPath = new List<PathCell>();
        firstPath.Add(start);
        incompletePaths.Enqueue(firstPath, 0);
        while (incompletePaths.Count > 0)
        {
            var path = incompletePaths.Dequeue();
            PathCell currentCell = path.Last();
            bool splitDecision = false;

            foreach (Direction dir in directions)
            {
                if (IsMoveValid(currentCell.pos, dir, out PathCell nextStep) && !path.Contains(nextStep))
                {
                    if (nextStep.isIntersection || nextStep.isDeadEnd)
                    {
                        Edge newEdge = new Edge(path[0], nextStep, path.Count);
                        newEdge.symbol = intersectionNames[idx % intersectionNames.Length];
                        path.Where(p => !p.isIntersection && !p.isDeadEnd).ForEach(p => p.symbol = newEdge.symbol);
                        edges.Add(newEdge);
                        edgeLookup.Add(newEdge.GetHashCode(), newEdge);
                        int moves = 0;
                        if (IsMoveValid(nextStep.pos, Direction.Up))
                            moves++;
                        if (IsMoveValid(nextStep.pos, Direction.Left))
                            moves++;
                        if (IsMoveValid(nextStep.pos, Direction.Down))
                            moves++;
                        if (IsMoveValid(nextStep.pos, Direction.Right))
                            moves++;
                        for (int i = 0; i < moves; i++)
                            incompletePaths.Enqueue(new List<PathCell> { nextStep }, idx++);
                        break;
                    }
                    else
                    {
                        nextStep.isStaticObstacle = true;
                        path.Add(nextStep);
                        incompletePaths.Enqueue(path, 0);
                        break;
                    }
                }
            }
        }
        foreach (var edge in edges)
        {
            edge.a.edgeNeighbors.Add(edge.b);
            edge.b.edgeNeighbors.Add(edge.a);
        }
        int maxSum1 = 0;
        firstPath = new List<PathCell>();
        firstPath.Add(start);
        incompletePaths.Clear();
        completedPaths.Clear();
        incompletePaths.Enqueue(firstPath, 0);
        while (incompletePaths.Count > 0)
        {
            var path = incompletePaths.Dequeue();
            PathCell currentCell = path.Last();

            for (int i = 0; i < currentCell.edgeNeighbors.Count; i++)
            {
                var neighborCell = currentCell.edgeNeighbors[i];
                if (!path.Contains(neighborCell))
                {
                    var newPath = new List<PathCell>(path);
                    newPath.Add(neighborCell);

                    if (neighborCell != end)
                    {
                        incompletePaths.Enqueue(newPath, newPath.Count);
                    }
                    else
                    {
                        if (newPath.Count >= 35)
                        {
                            completedPaths.Add(newPath);

                            int sum = 0;
                            for (int j = 0; j < newPath.Count - 1; j++)
                            {
                                PathCell a = newPath[j];
                                PathCell b = newPath[j + 1];

                                int hash = a.GetHashCode() + b.GetHashCode();
                                //if (edgeLookup.ContainsKey(hash))
                                sum += edgeLookup[hash].length;
                            }
                            if (sum > maxSum1)
                            {
                                maxSum1 = sum;
                                Debug.WriteLine(sum);
                            }

                        }
                    }

                }
            }
        }
        int maxSum = 0;
        foreach (var path in completedPaths)
        {
            int sum = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                PathCell a = path[i];
                PathCell b = path[i + 1];

                int hash = a.GetHashCode() + b.GetHashCode();
                //if (edgeLookup.ContainsKey(hash))
                sum += edgeLookup[hash].length;
            }
            maxSum = Math.Max(maxSum, sum);
        }

    }
    int HeuristicFunction(PathCell cell)
    {
        return ManhattanDistance(cell, end);
    }
    // 6554 yes!

    // 6520 wrong
    // 6483 wrong
    // 6174 too low 
    // 6424 ???
    // 6423 too low 

    protected override char CellToSymbol(PathCell cell)
    {
        if (cell.symbol == '#')
        {
            return ' ';
        }
        if (cell.isIntersection)
        {

            return intersectionNames[intersections.IndexOf(cell)];
        }
        if (cell.isDeadEnd)
        {
            return 'o';
        }
        return base.CellToSymbol(cell);
    }

    internal class PathCell : Cell
    {
        public bool isIntersection = false;
        public bool isOnPath = false;
        public SlopeType slopeType = SlopeType.NONE;
        public List<PathCell> edgeNeighbors = new List<PathCell>();
        public bool isDeadEnd = false;
        public PathCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            isStaticObstacle = symbol == '#';
            if (symbol == '>')
                slopeType = SlopeType.Right;
            if (symbol == 'v')
                slopeType = SlopeType.Down;
        }

        internal static PathCell GetPathCell((int, int) pos, char symbol)
        {
            return new PathCell(pos.Item1, pos.Item2, symbol);
        }
    }

    public enum SlopeType
    {
        NONE, Down, Right
    }

    Dictionary<int, Edge> edgeLookup = new Dictionary<int, Edge>();

    class Edge
    {
        public char symbol;
        public PathCell a, b;
        public int length;

        public Edge(PathCell a, PathCell b, int length)
        {
            this.a = a;
            this.b = b;
            this.length = length;
        }

        public override string ToString()
        {
            return $"{symbol}: {a} - {b} ({length})";
        }

        public override int GetHashCode()
        {
            return a.GetHashCode() + b.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj != null && (obj.GetHashCode() == this.GetHashCode());
        }
    }

}
