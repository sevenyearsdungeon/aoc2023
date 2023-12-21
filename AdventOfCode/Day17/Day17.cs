using System.Reflection;

internal class Day17 : MapSolution<Day17.HeatCell>
{
    (int, int) start = (0, 0);
    (int, int) goal => (mapWidth - 1, mapHeight - 1);

    public Day17()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        ReadMap(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt"), HeatCell.Build);
    }


    public override string SolvePart1()
    {
        MapPath path = new MapPath();
        path.moves = new List<Move>();
        path.moves.AddRange(GetMovesAt(start));
        return "";
    }

    public override string SolvePart2()
    {
        return "";
    }

    int Heuristic((int, int) position)
    {
        return Math.Abs(goal.Item2 - position.Item2) + Math.Abs(goal.Item1 - position.Item1);
    }

    public class HeatCell : Cell
    {
        public int pathCost = int.MaxValue;
        public readonly int heatLoss;

        public HeatCell(int x, int y, char symbol) : base(x, y, symbol)
        {
            heatLoss = symbol - 48;
        }
        public static HeatCell Build((int, int) position, char symbol)
        {
            return new HeatCell(position.Item1, position.Item2, symbol);
        }
        public override void Reset()
        {
            pathCost = int.MaxValue;
            base.Reset();
        }
    }
    public class MapPath
    {
        public List<Move> moves = new List<Move>();
        public int Cost => moves.Sum(m => m.moveCost);
    }

    public class Move
    {
        public int moveCost;
        public Direction direction;
    }

    List<Move> GetMovesAt((int,int) position)
    {
        List<Move> availableMoves = new List<Move> ();
        foreach (Direction direction in Enum.GetValues(typeof(Direction)))
        {
            var newPosition = GetNeighborPosition(position, direction);
            if (IsOnMap(newPosition))
                availableMoves.Add(new Move() { moveCost = GetCell(newPosition).heatLoss, direction = direction });
        }
        return availableMoves;
    }
}
