using AdventOfCode;
using CommunityToolkit.HighPerformance;
using System.Collections;
using System.Data;
using System.Text;

internal abstract class MapSolution<T> : Solution where T : Cell
{
    protected T[,]? map { get; private set; }
    protected int mapWidth { get; private set; }
    protected int mapHeight { get; private set; }
    protected ReadOnlySpan2D<T> Span { get => map; }

    protected T GetCell((int, int) position) => map[position.Item1, position.Item2];
    protected bool IsOnMap((int, int) pos) => (pos.Item1 >= 0 && pos.Item2 >= 0 && pos.Item1 < mapWidth && pos.Item2 < mapHeight);

    protected virtual void ReadMap(string path, Func<(int, int), char, T> cellProducer)
    {
        string[] lines = File.ReadAllLines(path);
        mapWidth = lines[0].Length;
        mapHeight = lines.Length;
        map = new T[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                char symbol = lines[y][x];
                T newCell = cellProducer.Invoke((x, y), symbol);
                map[x, y] = newCell;
            }
    }

    protected int ManhattanDistance(T a, T b)
    {
        return Math.Abs(b.y - a.y) + Math.Abs(a.x - b.x);
    }


    protected void ReadMapPadded(string path, Func<(int, int), char, T> cellProducer, int n, char skipChar)
    {
        string[] lines = File.ReadAllLines(path);
        int inputWidth = lines[0].Length;
        int inputHeight = lines.Length;
        int centerTileXMin = inputWidth * (n / 2);
        int centerTileXMax = inputWidth * (n / 2 + 1);
        int centerTileYMin = inputHeight * (n / 2);
        int centerTileYMax = inputHeight * (n / 2 + 1);
        mapWidth = n * inputWidth;
        mapHeight = n * inputHeight;
        map = new T[mapWidth, mapHeight];
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                bool centerTile = x > centerTileXMin && x < centerTileXMax && y > centerTileYMin && y < centerTileYMax;
                char symbol = lines[y % inputHeight][x % inputWidth];
                if (centerTile)
                {
                    int a = 0;
                }

                if (!centerTile && symbol == skipChar)
                    symbol = '.';
                T newCell = cellProducer.Invoke((x, y), symbol);
                map[x, y] = newCell;
            }
    }

    protected void Reset()
    {

        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, y].Reset();
            }
    }

    public static Direction[] directions = (Direction[])Enum.GetValues(typeof(Direction));
    public enum Direction
    {
        Up = 0,       // ^
        Right = 1,    // >
        Down = 2,     // v
        Left = 3,     // <
    }

    public static (int, int) GetDelta(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return (0, -1);
            case Direction.Down:
                return (0, 1);
            case Direction.Left:
                return (-1, 0);
            case Direction.Right:
                return (1, 0);
        }
        return (0, 0);
    }
    public (int, int) GetNeighborPosition((int, int) position, Direction direction)
    {
        var delta = GetDelta(direction);
        var newPosition = (position.Item1 + delta.Item1, position.Item2 + delta.Item2);

        return newPosition;
    }

    public T GetNeighborCell(T cell, Direction direction)
    {
        var delta = GetDelta(direction);
        var newPosition = (cell.pos.Item1 + delta.Item1, cell.pos.Item2 + delta.Item2);

        return GetCell(newPosition);
    }

    public virtual bool IsMoveValid((int, int) position, Direction direction)
    {
        var delta = GetDelta(direction);
        position.Item1 += delta.Item1;
        position.Item2 += delta.Item2;
        return IsOnMap(position) && !GetCell(position).isStaticObstacle;
    }
    public virtual bool IsMoveValid(T cell, Direction direction)
    {
        var delta = GetDelta(direction);
        (int, int) position = (cell.pos.Item1 + delta.Item1, cell.pos.Item2 + delta.Item2);
        return IsOnMap(position) && !GetCell(position).isStaticObstacle;
    }
    public virtual bool IsMoveValid(T cell, Direction direction, out T destinationCell)
    {
        destinationCell = null;
        var delta = GetDelta(direction);
        (int, int) position = (cell.pos.Item1 + delta.Item1, cell.pos.Item2 + delta.Item2);
        if (IsOnMap(position))
        {
            destinationCell = GetCell(position);
            return !destinationCell.isStaticObstacle;
        }
        return false;
    }

    public virtual bool IsMoveValid((int, int) position, Direction direction, out T destinationCell)
    {
        destinationCell = null;
        var delta = GetDelta(direction);
        position.Item1 += delta.Item1;
        position.Item2 += delta.Item2;
        if (IsOnMap(position))
        {
            destinationCell = GetCell(position);
            return !destinationCell.isStaticObstacle;
        }
        return false;
    }

    protected string GetMapString()
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                sb.Append(CellToSymbol(map[x, y]));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    protected virtual char CellToSymbol(T cell)
    {
        return cell.symbol;
    }

    protected bool FindCellBySymbol(char c, out (int, int) pos)
    {
        pos = (-1, -1);
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (map[x, y].symbol == c)
                {
                    pos = (x, y);
                    return true;
                }
            }
        }
        return false;
    }

    protected void ForEachCell(Action<T> action)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                action.Invoke(map[x, y]);
            }
        }
    }
}

