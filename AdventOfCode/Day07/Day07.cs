using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day07 : Solution
{

    static Dictionary<char, int> cardValues = new Dictionary<char, int>() {
        {'2',0},
        {'3',1},
        {'4',2},
        {'5',3},
        {'6',4},
        {'7',5},
        {'8',6},
        {'9',7},
        {'T',8},
        {'J',9},
        {'Q',10},
        {'K',11},
        {'A',12}};
    static List<string> lines;
    static List<Hand> hands;
    static Day07()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day07)}\\{nameof(Day07)}.txt")).ToList();
        hands = lines.Select(line =>
        {
            var entries = line.Split(' ');
            return new Hand(entries[0], int.Parse(entries[1]));
        }).ToList();
        hands.Sort(new HandComparer());
    }
    public override string SolvePart1()
    {
        throw new NotImplementedException();
    }

    public override string SolvePart2()
    {
        throw new NotImplementedException();
    }

    class Hand
    {
        public int bid;
        public string handString;
        public HandType handType;
        Dictionary<char, int> cardCounts = new Dictionary<char, int>();
        public Hand(string handString, int bid)
        {
            this.handString = handString;
            for (int i = 0; i < handString.Length; i++)
            {
                char c = handString[i];
                if (!cardCounts.ContainsKey(c))
                    cardCounts[c] = 0;
                cardCounts[c] = cardCounts[c] + 1;
            }
            DetermineHandType();
            this.bid = bid;
        }

        private void DetermineHandType()
        {
            if (cardCounts.Count == 1)
                handType = HandType.FiveKind;
            else if (cardCounts.Count == 2)
            {
                if (cardCounts.Values.Max(c => c) == 3)
                    handType = HandType.FullHouse;
                else
                    handType = HandType.FourKind;
            }
            else if (cardCounts.Count == 3)
            {
                if (cardCounts.Values.Max(c => c) == 3)
                    handType = HandType.ThreeKind;
                else
                    handType = HandType.TwoPair;
            }
            else if (cardCounts.Values.Max(c => c) == 2)
                handType = HandType.OnePair;
            else
                handType = HandType.Highest;
        }
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            int handCompare = x.handType.CompareTo(y.handType);
            int i = 0;
            while (handCompare != 0)
            {
                handCompare = x.handString[i].CompareTo(y.handString[i]);
                i++;
            }
            return handCompare;
        }
    }



    enum HandType
    {
        Highest,
        OnePair,
        TwoPair,
        ThreeKind,
        FullHouse,
        FourKind,
        FiveKind,
    }
}

