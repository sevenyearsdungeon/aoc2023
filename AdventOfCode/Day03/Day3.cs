using AdventOfCode;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

internal class Day3 : Solution
{
    static readonly int maxColumn, maxRow;
    static readonly Regex symbolRegex = new Regex("[^0-9.]");
    static readonly Regex numberRegex = new Regex("[0-9]{1,}");

    static readonly List<Symbol> symbols;
    static readonly Dictionary<(int, int), Number> numberDictionary;

    static Day3()
    {
        string[] lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\Day03\\{nameof(Day3)}.txt"));
        maxRow = lines.Length - 1;
        maxColumn = lines[0].Length - 1;
        symbols = new List<Symbol>();
        numberDictionary = new Dictionary<(int, int), Number>();
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            foreach (Match match in symbolRegex.Matches(line))
            {
                symbols.Add(new Symbol(match.Value, i, match.Index));
            }

            foreach (Match match in numberRegex.Matches(line))
            {
                int value = int.Parse(match.Value);
                Number newNumber = new Number(value);
                for (int j = 0; j < Math.Log10(value+0.1); j++)
                {
                    int row = i;
                    int col = match.Index + j;

                    numberDictionary.Add((row, col), newNumber);
                }
            }
        }

        foreach (Symbol symbol in symbols)
        {
            foreach ((int, int) index in Get8ConnectedIndices((symbol.rowIndex,symbol.columnIndex)))
            {
                if (numberDictionary.TryGetValue(index, out Number number))
                {
                    symbol.AddNumber(number);
                }
            }
        }
    }

    public override string SolvePart1()
    {
        return symbols.SelectMany(symbol => symbol.adjacentNumbers).Distinct().Sum(number => number.value).ToString();
    }

    public override string SolvePart2()
    {
        return symbols
            .Where(symbol => symbol.symbolValue.Equals("*"))
            .Where(symbol => symbol.adjacentNumbers.Count == 2)
            .Select(symbol =>
            {
                int product = 1;
                foreach (Number number in symbol.adjacentNumbers)
                {
                    product *= number.value;
                }
                return product;
            }            )
            .Sum(product=>product)
            .ToString();
    }

    class Symbol
    {
        public readonly string symbolValue;
        public readonly int rowIndex;
        public readonly int columnIndex;
        public readonly ISet<Number> adjacentNumbers;
        public Symbol(string symbolValue, int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
            this.symbolValue = symbolValue;
            adjacentNumbers = new HashSet<Number>();
        }
        public void AddNumber(Number number)
        {
            adjacentNumbers.Add(number);
        }

        public override string ToString()
        {
            return $"{symbolValue}   :   {string.Join(", ", adjacentNumbers)}";
        }
    }

    class Number
    {
        public readonly int value;

        public Number(int value)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    public static List<(int, int)> Get8ConnectedIndices((int, int) index)
    {
        var result = new List<(int, int)>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;
                int row = index.Item1 + i;
                int col = index.Item2 + j;
                if (row >= 0 && row <= maxRow && col >= 0 && col <= maxColumn)
                    result.Add((row, col));
            }
        }
        return result;
    }
}

