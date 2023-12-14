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
    static Day07()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day07)}\\{nameof(Day07)}.txt")).ToList();

    }
    public override string SolvePart1()
    {
        List<Hand> hands = lines.Select(line =>
        {
            var entries = line.Split(' ');
            return new Hand(entries[0], int.Parse(entries[1]), false);
        }).ToList();
        hands.Sort();

        int sum = 0;
        for (int i = 0; i < hands.Count; i++)
        {
            sum += (i + 1) * hands[i].bid;
        }
        return sum.ToString();
    }

    public override string SolvePart2()
    {
        cardValues['J'] = -100;
        List<Hand> hands = lines.Select(line =>
        {
            var entries = line.Split(' ');
            return new Hand(entries[0], int.Parse(entries[1]), true);
        }).ToList();
        hands.Sort();

        int sum = 0;
        for (int i = 0; i < hands.Count; i++)
        {
            sum += (i + 1) * hands[i].bid;
        }
        return sum.ToString();
    }

    class Hand : IComparable
    {
        public int bid;
        private readonly bool useWildcards;
        public string handString;
        public HandType handType;
        Dictionary<char, int> cardCounts = new Dictionary<char, int>();
        public Hand(string handString, int bid, bool useWildcards)
        {
            int wildCardCount = 0;
            int cardCount = 0;
            this.handString = handString;
            for (int i = 0; i < handString.Length; i++)
            {
                char c = handString[i];
                if (useWildcards && c == 'J')
                    wildCardCount++;
                else
                {
                    cardCount++;
                    if (!cardCounts.ContainsKey(c))
                        cardCounts[c] = 0;
                    cardCounts[c] = cardCounts[c] + 1;
                }
            }
            DetermineHandType(cardCount);
            if (useWildcards && wildCardCount>0)
            {
                handType = UpdateHandWithWildcards(wildCardCount);
            }
            this.bid = bid;
            this.useWildcards = useWildcards;
        }

        private HandType UpdateHandWithWildcards(int wildCount)
        {
            switch (handType)
            {
                case HandType.Highest:
                    if (wildCount >= 4)
                        return HandType.FiveKind;
                    if (wildCount == 3)
                        return HandType.FourKind;
                    if (wildCount == 2)
                        return HandType.ThreeKind;
                    if (wildCount == 1)
                        return HandType.OnePair;
                    break;

                case HandType.OnePair:
                    if (wildCount == 3)
                        return HandType.FiveKind;
                    if (wildCount == 2)
                        return HandType.FourKind;
                    if (wildCount == 1)
                        return HandType.ThreeKind;
                    if (wildCount == 0)
                        return handType;
                    break;

                case HandType.TwoPair:
                    if (wildCount == 1)
                        return HandType.FullHouse;
                    break;

                case HandType.ThreeKind:
                    if (wildCount == 2)
                        return HandType.FiveKind;
                    if (wildCount == 1)
                        return HandType.FourKind;
                    break;

                case HandType.FullHouse:
                    break;

                case HandType.FourKind:
                    if (wildCount == 1)
                        return HandType.FiveKind;
                    break;

                case HandType.FiveKind:
                    break;
            }
            return handType;
        }

        public int CompareTo(object? obj)
        {
            Hand otherHand = obj as Hand;
            int handCompare = handType.CompareTo(otherHand.handType);
            int i = 0;
            while (handCompare == 0)
            {
                handCompare = cardValues[handString[i]].CompareTo(cardValues[otherHand.handString[i]]);
                i++;
            }
            return handCompare;
        }

        private void DetermineHandType(int totalCards)
        {
            int distinctCards = cardCounts.Count;
            if (totalCards == 5)
            {
                if (distinctCards == 1)
                    handType = HandType.FiveKind;
                else if (distinctCards == 2)
                {
                    if (cardCounts.Values.Max(c => c) == 3)
                        handType = HandType.FullHouse;
                    else
                        handType = HandType.FourKind;
                }
                else if (distinctCards == 3)
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
            else if (totalCards == 4)
            {
                if (distinctCards == 1)
                    handType = HandType.FourKind;
                else if (distinctCards == 2)
                {
                    if (cardCounts.Values.Max(c => c) == 3)
                        handType = HandType.ThreeKind;
                    else
                        handType = HandType.TwoPair;
                }
                else if (distinctCards == 3)
                {
                    handType = HandType.OnePair;
                }
                else
                    handType = HandType.Highest;
            }
            else if (totalCards == 3)
            {
                if (distinctCards == 1)
                    handType = HandType.ThreeKind;
                else if (distinctCards == 2)
                {
                    handType = HandType.OnePair;
                }
                else
                    handType = HandType.Highest;
            }
            else if (totalCards == 2)
            {
                if (distinctCards == 1)
                    handType = HandType.OnePair;
                else
                    handType = HandType.Highest;
            }
            else
                handType = HandType.Highest;
        }

        public override string ToString()
        {
            return handString;
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

