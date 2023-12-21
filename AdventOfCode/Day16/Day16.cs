using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

internal class Day16 : Solution
{
    static List<string> lines;
    static readonly MirrorCell[,] map;
    static Stack<Beam> beams = new Stack<Beam>();

    static MirrorCell GetCell((int, int) position) => map[position.Item1, position.Item2];

    static readonly int mapWidth, mapHeight;
    static Day16()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt")).ToList(); mapWidth = lines[0].Length;
        mapHeight = lines.Count;
        map = new MirrorCell[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                char symbol = lines[mapHeight - y - 1][x];
                MirrorCell newCell = new MirrorCell(x, y, symbol);
                map[x, y] = newCell;
            }
    }

    public void PrintEnergyMap()
    {
        File.WriteAllText(@"C:\Users\LombardoNick\source\repos\AdventOfCode\AdventOfCode\Day16\Day16energy.txt", GetMapString());
    }

    static string GetMapString()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = mapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                sb.Append(map[x, y].energized ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public override string SolvePart1()
    {
        beams.Push(new Beam() { currentDirection = Direction.Right, position = (0, mapHeight - 1) });
        while (Propagate()) { }

        //   PrintEnergyMap();
        return CountEnergized().ToString();
    }

    public override string SolvePart2()
    {
        int maxEnergy = 0;
        for (int x = 0; x < mapWidth; x++)
        {
            beams.Push(new Beam() { currentDirection = Direction.Down, position = (x, mapHeight - 1) });
            while (Propagate()) { }
            maxEnergy = Math.Max(maxEnergy, CountEnergized());
            ResetMap();

            beams.Push(new Beam() { currentDirection = Direction.Up, position = (x, 0) });
            while (Propagate()) { }

            maxEnergy = Math.Max(maxEnergy, CountEnergized());
            ResetMap();
        }
        for (int y = 0; y < mapWidth; y++)
        {
            beams.Push(new Beam() { currentDirection = Direction.Right, position = (0, y) });
            while (Propagate()) { }
            maxEnergy = Math.Max(maxEnergy, CountEnergized());
            ResetMap();

            beams.Push(new Beam() { currentDirection = Direction.Left, position = (mapWidth-1, 0) });
            while (Propagate()) { }
            maxEnergy = Math.Max(maxEnergy, CountEnergized());
            ResetMap();
        }
        return maxEnergy.ToString();
    }

    void ResetMap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y].Reset();

            }
        }
    }

    class MirrorCell : Cell
    {
        public bool energized = false;
        public readonly OpticalElement opticalElement;

        HashSet<Direction> incomingDirections = new HashSet<Direction>();
        HashSet<Direction> outgoingDirections = new HashSet<Direction>();

        public bool HasEvaluatedIncoming(Direction direction) => incomingDirections.Contains(direction);
        public bool HasEvaluatedOutgoing(Direction direction) => outgoingDirections.Contains(direction);
        public void SetEvaluatedIncoming(Direction direction) => incomingDirections.Add(direction);
        public void SetEvaluatedOutgoing(Direction direction) => outgoingDirections.Add(direction);

        internal void Reset()
        {
            incomingDirections.Clear();
            outgoingDirections.Clear();
            energized = false;
        }

        public MirrorCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            opticalElement = (OpticalElement)symbol;
        }
    }

    static int CountEnergized()
    {
        int sum = 0;
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                sum += map[x, y].energized ? 1 : 0;
            }
        }
        return sum;
    }
    class Beam
    {
        public Direction currentDirection;
        public (int, int) position;

        public bool MoveForward()
        {
            var delta = GetDelta(currentDirection);
            position.Item1 += delta.Item1;
            position.Item2 += delta.Item2;
            return IsOnMap(position);

        }

        public Beam GetCopy()
        {
            return new Beam() { currentDirection = currentDirection, position = position };
        }

        public override string ToString()
        {
            return $"{currentDirection.ToString()}, ({position.Item1},{position.Item2})";
        }
    }

    static bool Propagate()
    {
        if (beams.Count == 0)
            return false;

        Beam beam = beams.Peek();
        MirrorCell cell = GetCell(beam.position);

        if (cell.HasEvaluatedIncoming(beam.currentDirection))
        {
            beams.Pop();
            return (beams.Count > 0);
        }
        cell.energized = true;
        cell.SetEvaluatedIncoming(beam.currentDirection);

        switch (cell.opticalElement)
        {
            case OpticalElement.Empty:
                return MoveFirstBeamForward(beams, cell);

            case OpticalElement.HorizontalSplitter:
                if (beam.currentDirection == Direction.Left || beam.currentDirection == Direction.Right)
                    return MoveFirstBeamForward(beams, cell);
                else
                    return SplitFirstBeam(beams, cell);
            case OpticalElement.VerticalSplitter:
                if (beam.currentDirection == Direction.Up || beam.currentDirection == Direction.Down)
                    return MoveFirstBeamForward(beams, cell);
                else
                    return SplitFirstBeam(beams, cell);
            case OpticalElement.ForwardMirror:
            case OpticalElement.BackwardMirror:
                return ReflectBeam(beams, cell);
            default:
                break;
        }
        return true;
    }

    private static bool ReflectBeam(Stack<Beam> beams, MirrorCell cell)
    {
        Beam beam = beams.Pop();
        switch (beam.currentDirection)
        {
            case Direction.Up:
                if (cell.opticalElement == OpticalElement.ForwardMirror)
                    beam.currentDirection = Direction.Right;
                else
                    beam.currentDirection = Direction.Left;
                break;
            case Direction.Down:
                if (cell.opticalElement == OpticalElement.ForwardMirror)
                    beam.currentDirection = Direction.Left;
                else
                    beam.currentDirection = Direction.Right;
                break;
            case Direction.Left:
                if (cell.opticalElement == OpticalElement.ForwardMirror)
                    beam.currentDirection = Direction.Down;
                else
                    beam.currentDirection = Direction.Up;
                break;
            case Direction.Right:
                if (cell.opticalElement == OpticalElement.ForwardMirror)
                    beam.currentDirection = Direction.Up;
                else
                    beam.currentDirection = Direction.Down;
                break;
        }
        if (!cell.HasEvaluatedOutgoing(beam.currentDirection))
        {
            beams.Push(beam);
            MoveFirstBeamForward(beams, cell);
        }
        return beams.Count > 0;
    }

    private static bool SplitFirstBeam(Stack<Beam> beams, MirrorCell cell)
    {
        Beam beam = beams.Pop();
        Beam beam1, beam2;
        if (beam.currentDirection == Direction.Left || beam.currentDirection == Direction.Right)
        {
            beam1 = new Beam { currentDirection = Direction.Up, position = beam.position };
            beam2 = new Beam { currentDirection = Direction.Down, position = beam.position };
        }
        else
        {
            beam1 = new Beam { currentDirection = Direction.Right, position = beam.position };
            beam2 = new Beam { currentDirection = Direction.Left, position = beam.position };
        }
        if (!cell.HasEvaluatedOutgoing(beam1.currentDirection))
        {
            beams.Push(beam1);
            MoveFirstBeamForward(beams, cell);
        }
        if (!cell.HasEvaluatedOutgoing(beam2.currentDirection))
        {
            beams.Push(beam2);
            MoveFirstBeamForward(beams, cell);
        }
        return beams.Count > 0;
    }

    private static bool MoveFirstBeamForward(Stack<Beam> beams, MirrorCell cell)
    {
        Beam beam = beams.Pop();
        if (!cell.HasEvaluatedOutgoing(beam.currentDirection))
        {
            cell.SetEvaluatedOutgoing(beam.currentDirection);
            if (beam.MoveForward())
                beams.Push(beam);
        }
        return (beams.Count > 0);
    }


    public enum Direction
    {
        Up = 0,            // ^
        Down = 1,     // v
        Left = 2,     // <
        Right = 3,    // >
    }

    static public (int, int) GetDelta(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return (0, 1);
            case Direction.Down:
                return (0, -1);
            case Direction.Left:
                return (-1, 0);
            case Direction.Right:
                return (1, 0);
        }
        return (0, 0);
    }

    static public bool IsOnMap((int, int) pos)
    {
        return (pos.Item1 >= 0 && pos.Item2 >= 0 && pos.Item1 < mapWidth && pos.Item2 < mapHeight);
    }

    public enum OpticalElement
    {
        Empty = '.',
        HorizontalSplitter = '-',
        VerticalSplitter = '|',
        ForwardMirror = '/',
        BackwardMirror = '\\',
    }
}