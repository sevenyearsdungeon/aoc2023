using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day12 : Solution
{
    static List<Puzzle> puzzles = new List<Puzzle>();
    static Day12()
    {
        Queue<string> lines = new Queue<string>(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\day12\\day12.txt")).ToList());
        while (lines.Count > 0)
            puzzles.Add(new Puzzle(lines.Dequeue()));
    }

    public override string SolvePart1()
    {
        return puzzles.Select(p => p.solutions.Count).Sum().ToString();
    }

    public override string SolvePart2()
    {
        return "";
        puzzles.Clear();
        Queue<string> lines = new Queue<string>(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\day12\\day12.txt")).ToList());
            StringBuilder builder = new StringBuilder();
        while (lines.Count > 0)
        {
            builder.Clear();
            var line = lines.Dequeue();
            var parts = line.Split(' ');
            for (int i = 0; i < 5; i++)
            {
                builder.Append(parts[0]);
            }
            builder.Append(" ");
            for (int i = 0; i < 5; i++)
            {
                builder.Append(parts[1]);
                if (i < 4)
                    builder.Append(",");
            }
            puzzles.Add(new Puzzle(builder.ToString()));
        }

        return puzzles.Select(p => p.solutions.Count).Sum().ToString();
    }

    public class Puzzle
    {
        readonly public string puzzleString;
        readonly int puzzleLength;
        public readonly List<int> blockLengths;
        public readonly List<string> combinations;
        public readonly List<string> solutions = new List<string>();
        List<List<int>> permutations = new List<List<int>>();

        public Puzzle(string line)
        {
            string[] entries = line.Split(' ');
            puzzleString = entries[0];
            puzzleLength = puzzleString.Length;
            blockLengths = entries[1].Split(',').Select(num => int.Parse(num)).ToList();

            combinations = new List<string>();
            Queue<int> blockQueue = new Queue<int>(blockLengths);

            StringBuilder builder = new StringBuilder();
            List<int> insertionPoints = new List<int>();
            int currentBlockOffset = 0;
            foreach (int i in blockLengths)
            {
                insertionPoints.Add(currentBlockOffset);
                currentBlockOffset += i + 1;
            }
            while (blockQueue.Count > 0)
            {
                int i = blockQueue.Dequeue();
                for (int j = 0; j < i; j++)
                    builder.Append("#");
                builder.Append(".");
            }
            int slack = puzzleLength - builder.Length + 1;

            while (builder.Length < puzzleLength)
            {
                builder.Append(".");
            }

            string candidateSolution = builder.ToString();

            List<int> basePermutation = insertionPoints.Select(i => 0).ToList();
            permutations.Add(basePermutation);
            for (int insertionIndex = 0; insertionIndex < insertionPoints.Count; insertionIndex++)
            {
                for (int insertionValue = 1; insertionValue <= slack; insertionValue++)
                {
                    for (int i = 0; i < permutations.Count; i++)
                    {
                        var currentPermutation = permutations[i];
                        if (currentPermutation.Sum(p => p) + insertionValue <= slack && currentPermutation[insertionIndex] == 0)
                        {
                            var newPermutation = new List<int>(currentPermutation);
                            newPermutation[insertionIndex] = insertionValue;
                            permutations.Add(newPermutation);
                        }
                    }
                }
            }

            foreach (List<int> permutation in permutations)
            {
                builder.Clear();
                for (int i = 0; i < permutation.Count; i++)
                {
                    int insertionCount = permutation[i];
                    for (int j = 0; j < insertionCount; j++)
                        builder.Append(".");
                    for (int j = 0; j < blockLengths[i]; j++)
                        builder.Append("#");
                    builder.Append(".");


                }
                while (builder.Length < puzzleLength)
                {
                    builder.Append(".");
                }

                candidateSolution = builder.ToString();
                if (CheckSolution(candidateSolution))
                    solutions.Add(candidateSolution);
            }

        }

        bool CheckSolution(string solution)
        {
            for (int i = 0; i < puzzleLength; i++)
            {
                char a = puzzleString[i];
                char b = solution[i];
                if (a != b && a != '?')
                    return false;
            }
            return true;
        }

    }
}

