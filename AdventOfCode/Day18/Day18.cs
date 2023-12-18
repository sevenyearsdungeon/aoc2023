using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


internal class Day18 : Solution
{
    static List<string> lines;

    static List<Step> steps = new List<Step>();
    static long mapWidth = 0;
    static long mapHeight = 0;

    static char[,] map;
    static string mapString;
    static long enclosed = 0;
    static long perimeter = 0;
    static Day18()
    {
        string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{className}\\{className}.txt")).ToList();
        long minx = 0, maxx = 0, miny = 0, maxy = 0;
        long x = 1;
        long y = 0;
        foreach (string line in lines)
        {
            var entries = line.Split(' ');
            char direction = entries[0][0];
            long distance = long.Parse(entries[1]);
            string colorString = entries[2].Substring(2, 6);
            Step step = new Step(direction, distance, colorString);
            steps.Add(step);
            switch (step.direction)
            {
                case 'R':
                    x += step.distance;
                    maxx = Math.Max(x, maxx);
                    break;
                case 'L':
                    x -= step.distance;
                    minx = Math.Min(x, minx);
                    break;
                case 'D':
                    y += step.distance;
                    maxy = Math.Max(y, maxy);
                    break;
                case 'U':
                    y -= step.distance;
                    miny = Math.Min(y, miny);
                    break;
                default:
                    break;
            }
        }
        long sum = 0;

        long startx = -minx;
        long starty = -miny;
        long height = 1;

        bool firstR = true;
        for (int i = 0; i < steps.Count; i++)
        {
            Step currentStep = steps[i];

            char a = currentStep.direction;
            long delta = 0;
            if (a == 'R')
            {
                delta = currentStep.distance * (height);
            }
            if (a == 'L')
            {
                delta = -currentStep.distance * (height - 1);
            }
            if (a == 'U')
                height += currentStep.distance;
            if (a == 'D')
            {
                height -= currentStep.distance;
                delta = currentStep.distance;
            }
            sum += delta;
        }
        sum++;

        mapWidth = maxx - minx;
        mapHeight = maxy - miny + 1;

        if (mapWidth > 1000)
            return;


        map = new char[mapWidth, mapHeight];
        for (x = 0; x < mapWidth; x++)
            for (y = 0; y < mapHeight; y++)
            {
                map[x, y] = '.';
            }
        x = startx;
        y = starty;
        bool first = true;
        for (int i = 0; i < steps.Count; i++)
        {
            Step step = steps[i];
            long deltax = 0;
            long deltay = 0;
            char direction = step.direction;
            long distance = step.distance;
            char nextCornerChar = '~';
            char nextDirection = steps[(i + 1) % steps.Count].direction;
            switch (direction)
            {
                case 'R':
                    if (nextDirection == 'U')
                        nextCornerChar = 'J';
                    else nextCornerChar = '7';
                    deltax = 1;
                    break;
                case 'L':
                    if (nextDirection == 'U')
                        nextCornerChar = 'L';
                    else nextCornerChar = 'F';
                    deltax = -1;
                    break;
                case 'D':

                    if (nextDirection == 'R')
                        nextCornerChar = 'L';
                    else nextCornerChar = 'J';
                    deltay = 1;
                    break;
                case 'U':
                    if (nextDirection == 'R')
                        nextCornerChar = 'F';
                    else nextCornerChar = '7';
                    deltay = -1;
                    break;
                default:
                    break;
            }
            char lineChar = deltay == 0 ? '-' : '|';

            for (long j = 0; j < distance; j++)
            {
                x += deltax;
                y += deltay;
                if (first)
                {
                    map[x, y] = '%';
                    first = false;
                }
                else
                {
                    if (j == distance - 1)
                        map[x, y] = nextCornerChar;
                    else
                        map[x, y] = lineChar;
                }
                perimeter++;
            }
            nextDirection = direction;
        }

        File.WriteAllText(@"C:\Users\LombardoNick\source\repos\AdventOfCode\AdventOfCode\Day18\day18map.txt", GetMapString());
        enclosed = CountEnclosedPositions();
    }

    static string GetMapString()
    {
        StringBuilder sb = new StringBuilder();
        for (long y = 0; y < mapHeight; y++)
        {
            for (long x = 0; x < mapWidth; x++)
            {
                sb.Append(map[x, y]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }



    public override string SolvePart1()
    {
        return (enclosed + perimeter).ToString();
    }

    public override string SolvePart2()
    {
        return "";
    }

    class Step
    {
        public char direction;
        public long distance;
        public string colorString;

        public Step(char direction, long distance, string colorString)
        {
            this.direction = direction;
            this.distance = distance;
            this.colorString = colorString;
            UpdateFromColor();
        }

        void UpdateFromColor()
        {
            Direction dir = (Direction)long.Parse(colorString.Substring(colorString.Length - 1, 1));
            distance = long.Parse(colorString.Substring(0, 5), NumberStyles.HexNumber);
            switch (dir)
            {
                case Direction.Right:
                    this.direction = 'R';
                    break;
                case Direction.Down:
                    this.direction = 'D';
                    break;
                case Direction.Left:
                    this.direction = 'L';
                    break;
                case Direction.Up:
                    this.direction = 'U';
                    break;
            }
        }
    }

    enum Direction
    {
        Right, Down, Left, Up,
    }

    public static long CountEnclosedPositions()
    {
        long sum = 0;
        bool partOfLoop = false;


        for (long y = 0; y < mapHeight; y++)
        {
            bool insideLoop = false;
            bool hasSouthInput = false;
            bool hasNorthInput = false;
            for (long x = 0; x < mapWidth; x++)
            {

                char current = map[x, y];
                partOfLoop = (current != '.');
                if (partOfLoop)
                {
                    if (current == '|')
                    {
                        insideLoop = !insideLoop;
                        continue;
                    }
                    if (!hasSouthInput && !hasNorthInput)
                    {
                        if (current == 'F')
                        {
                            hasSouthInput = true;
                        }
                        if (current == 'J')
                        {
                            hasNorthInput = true;
                        }
                        if (current == '7')
                        {
                            hasSouthInput = true;
                        }
                        if (current == 'L')
                        {
                            hasNorthInput = true;
                        }
                        continue;
                    }
                    if (hasSouthInput)
                    {
                        if (current == 'F' || current == '7')
                        {
                            hasSouthInput = false;
                        }
                        if (current == 'J' || current == 'L')
                        {
                            hasSouthInput = false;
                            insideLoop = !insideLoop;
                        }
                    }
                    if (hasNorthInput)
                    {
                        if (current == 'F' || current == '7')
                        {
                            hasNorthInput = false;
                            insideLoop = !insideLoop;
                        }
                        if (current == 'J' || current == 'L')
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
                        map[x, y] = 'o';
                    }
                }


            }
        }
        return sum;
    }
}
