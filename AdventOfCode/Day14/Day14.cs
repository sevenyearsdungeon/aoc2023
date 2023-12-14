using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

internal class Day14 : Solution
{
    static List<string> lines;
    static List<RollingRock> rollingRocks = new List<RollingRock>();
    static Day14()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\day14\\day14.txt")).ToList();
        Regex regex = new Regex("O");

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            foreach (Match rock in regex.Matches(line))
            {
                RollingRock newRock = new RollingRock((rock.Index, i));
                rollingRocks.Add(newRock);
            }
        }
    }

    public override string SolvePart1()
    {

        return rollingRocks
            .Sum(r => lines.Count - r.northStopPosition.Item2)
            .ToString();
    }

    public override string SolvePart2()
    {
        return "";
    }

    class RollingRock
    {
        public (int, int) initialPosition;
        public (int, int) northStopPosition;

        public RollingRock((int, int) initialPosition)
        {
            this.initialPosition = initialPosition;
            northStopPosition = CalculateStopPosition(initialPosition,(0,-1));
            lines[initialPosition.Item2] = lines[initialPosition.Item2].Remove(initialPosition.Item1, 1).Insert(initialPosition.Item1, ".");
            lines[northStopPosition.Item2] = lines[northStopPosition.Item2].Remove(northStopPosition.Item1, 1).Insert(northStopPosition.Item1, "O");
        }

        (int, int) CalculateStopPosition((int, int) currentPosition, (int, int) direction)
        {
            int deltaX = direction.Item1;
            int deltaY = direction.Item2;

            int x = currentPosition.Item1;
            int y = currentPosition.Item2;
            if (y == 0 || y==lines.Count-1)
                return currentPosition;
            if (lines[y + deltaY][x + deltaX] != '.')
                return currentPosition;
            return CalculateStopPosition((x + deltaX, y + deltaY),direction);
        }

    }
}

