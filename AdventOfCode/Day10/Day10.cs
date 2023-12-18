
using AdventOfCode;

internal class Day10 : Solution
{
    static List<string> lines;
    static Pipe[,] maze;
    static HashSet<Pipe> loopPipes = new HashSet<Pipe>();
    static int loopLength = 1;
    static int startX, startY, mazeWidth, mazeHeight;

    static Day10()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day10)}\\{nameof(Day10)}.txt")).ToList();
         mazeWidth = lines[0].Length;
         mazeHeight = lines.Count;
        maze = new Pipe[mazeWidth, mazeHeight];
        for (int y = 0; y < mazeHeight; y++)
            for (int x = 0; x < mazeWidth; x++)
            {
                char symbol = lines[mazeHeight - y - 1][x];
                if (symbol == 'S')
                {
                    startX = x;
                    startY = y;
                    symbol = 'L'; // L
                }

                maze[x, y] = new Pipe(x, y, symbol);
            }

        BuildPipeMap();
    }


    public override string SolvePart1()
    {
        return (loopLength / 2).ToString();
    }

    public override string SolvePart2()
    {
        return CountEnclosedPositions().ToString();
    }



    public static int CountEnclosedPositions()
    {
        int sum = 0;
        bool partOfLoop = false;


        for (int y = 0; y < mazeHeight; y++)
        {
            bool insideLoop = false;
            bool hasSouthInput = false;
            bool hasNorthInput = false;
            for (int x = 0; x < mazeWidth; x++)
            {

                var current = maze[x, y];
                partOfLoop = (current.nextPipe != null);
                if (partOfLoop)
                {
                    if (current.symbol == '|')
                    {
                        insideLoop = !insideLoop;
                        continue;
                    }
                    if (!hasSouthInput && !hasNorthInput)
                    {
                        if (current.symbol == 'F')
                        {
                            hasSouthInput = true;
                        }
                        if (current.symbol == 'J')
                        {
                            hasNorthInput = true;
                        }
                        if (current.symbol == '7')
                        {
                            hasSouthInput = true;
                        }
                        if (current.symbol == 'L')
                        {
                            hasNorthInput = true;
                        }
                        continue;
                    }
                    if (hasSouthInput)
                    {
                        if (current.symbol == 'F' || current.symbol == '7')
                        {
                            hasSouthInput = false;
                        }
                        if (current.symbol == 'J' || current.symbol == 'L')
                        {
                            hasSouthInput = false;
                            insideLoop = !insideLoop;
                        }
                    }
                    if (hasNorthInput)
                    {
                        if (current.symbol == 'F' || current.symbol == '7')
                        {
                            hasNorthInput = false;
                            insideLoop = !insideLoop;
                        }
                        if (current.symbol == 'J' || current.symbol == 'L')
                        {
                            hasNorthInput = false;
                        }
                    }
                }
                else
                {
                    if (insideLoop)
                    {
                        sum++;
                    }
                }


            }
        }
        return sum;
    }

    private static void BuildPipeMap()
    {

        Pipe startPipe = maze[startX, startY];
        loopPipes.Add(startPipe);
        Direction travelDireciton = Direction.North; // North
        Pipe currentPipe = startPipe;
        do
        {
            loopLength++;
            int deltaX = 0, deltaY = 0;
            switch (travelDireciton)
            {
                case Direction.North:
                    deltaY++;
                    break;
                case Direction.South:
                    deltaY--;
                    break;
                case Direction.East:
                    deltaX++;
                    break;
                case Direction.West:
                    deltaX--;
                    break;
            }
            currentPipe.nextPipe = maze[currentPipe.x + deltaX, currentPipe.y + deltaY];

            switch (currentPipe.nextPipe.symbol)
            {
                case '-':
                    break;
                case '|':
                    break;
                case 'F':
                    if (travelDireciton == Direction.West)
                        travelDireciton = Direction.South;
                    else
                        travelDireciton = Direction.East;
                    break;
                case 'L':
                    if (travelDireciton == Direction.West)
                        travelDireciton = Direction.North;
                    else
                        travelDireciton = Direction.East;
                    break;
                case 'J':
                    if (travelDireciton == Direction.East)
                        travelDireciton = Direction.North;
                    else
                        travelDireciton = Direction.West;
                    break;
                case '7':
                    if (travelDireciton == Direction.East)
                        travelDireciton = Direction.South;
                    else
                        travelDireciton = Direction.West;
                    break;
            }

            currentPipe.nextPipe.previousPipe = currentPipe;
            currentPipe = currentPipe.nextPipe;
            loopPipes.Add(currentPipe);
        }
        while (currentPipe != startPipe);
    }

    class Pipe
    {
        public int x, y;
        public char symbol;
        public Pipe nextPipe;
        public Pipe previousPipe;

        public Pipe(int x, int y, char symbol)
        {
            this.x = x;
            this.y = y;
            this.symbol = symbol;
            this.previousPipe = previousPipe;
        }

        public override string ToString()
        {
            return symbol.ToString();
        }
    }


    enum Direction
    {
        North, South, East, West
    }
}
