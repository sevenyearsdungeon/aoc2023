using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day04 : Solution
{
    private const string s1 = ",,,,,,";
    private const string s2 = "a,,,,,,";
    private const string s3 = "a,,,,,,a";
    static List<string> lines;
    static Day04()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day04)}\\{nameof(Day04)}.txt")).ToList();
        foreach (var line in lines)
        {
            var s1 = line.Split(":");
            int index = int.Parse(s1[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
            var s2 = s1[1].Split("|");
            cards.Add(new Card(index, s2[0], s2[1]));
        }
    }

    static List<Card> cards = new List<Card>();

    public override string SolvePart1()
    {
        return cards.Select(card => card.GetValue()).Sum().ToString();
    }

    public override string SolvePart2()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int matchCount = cards[i].matchCount;
            int copies = cards[i].copies;
            for (int j = 0; j < matchCount; j++)
            {
                if (i + j + 1 < cards.Count)
                    cards[i + j + 1].copies += copies;
            }
        }
        return cards.Sum(card=>card.copies).ToString();
    }

    class Card
    {
        HashSet<int> winningNumbers;
        HashSet<int> cardNumbers;
        int index;
        public int matchCount;
        public int copies = 1;
        public Card(int index, string winningNumberString, string cardNumberString)
        {
            this.index = index;
            winningNumbers = winningNumberString
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s))
                .ToHashSet();
            cardNumbers = cardNumberString
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s))
                .ToHashSet();

            matchCount = winningNumbers.Intersect(cardNumbers).Count();
        }
        public void AddCopy()
        {
            copies++;
        }
        public long GetValue()
        {
            return matchCount > 0 ? (long)Math.Pow(2, matchCount - 1) : 0;
        }
    }

}

