

using AdventOfCode;

class Day2 : Solution
{
    static readonly string[] lines;
    static Day2()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\Day02\\day2.txt"));
    }


    public override string SolvePart1()
    {
        int sum = 0;
        foreach (var line in lines) 
        {
            Game currentGame = new Game(line);
            if (currentGame.check(12,13,14))
            {
                sum+=currentGame.index;
            }
        }
        return sum.ToString();
    }

    public override string SolvePart2()
    {
        int sum = 0;
        foreach (var line in lines)
        {
            Game currentGame = new Game(line);
            sum += currentGame.getPower();
            
        }
        return sum.ToString();
    }


    class Game
    {
        public readonly int index;
        public readonly List<Round> rounds;
        public Game(string line)
        {
            string[] entries = line.Split(":");
            index = int.Parse(entries[0].Split(' ')[1]);

            rounds = entries[1].Split(";")
            .Select(gameString => new Round(gameString))
            .ToList();
        }

        public bool check(int maxRed, int maxGreen, int maxBlue)
        {
            return rounds.All(round=>round.check(maxRed, maxGreen, maxBlue));
        }

        internal int getPower()
        {
            return rounds.Max(round => round.redCount) *
            rounds.Max(round => round.greenCount) *
            rounds.Max(round => round.blueCount);
        }
    }

    class Round
    {
        public Round(string gameLine)
        {
            var picks = gameLine.Split(",");
            foreach (var pick in picks)
            {
                var entries = pick.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                int count = int.Parse(entries[0].Trim());
                switch (entries[1].Trim().ToLower())
                {
                    case "red":
                        redCount = count;
                        break;
                    case "blue":
                        blueCount= count;
                        break;
                    case "green":
                        greenCount = count;
                        break;
                    default: break;
                }
            }

        }

        readonly public int redCount;
        readonly public int greenCount;
        readonly public int blueCount;

        public bool check(int maxRed, int maxGreen, int maxBlue)
        {
            return blueCount <= maxBlue && redCount <= maxRed && greenCount <= maxGreen;
        }
    }
}