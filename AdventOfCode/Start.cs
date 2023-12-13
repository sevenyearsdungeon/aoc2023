
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    internal class Start
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 1, Part 1: " + new Day1().SolvePart1());
            Console.WriteLine("Day 1, Part 2: " + new Day1().SolvePart2());

            Console.WriteLine("Day 2, Part 1: " + new Day2().SolvePart1());
            Console.WriteLine("Day 2, Part 2: " + new Day2().SolvePart2());

            Console.WriteLine("Day 3, Part 1: " + new Day3().SolvePart1());
            Console.WriteLine("Day 3, Part 2: " + new Day3().SolvePart2());

            Console.WriteLine("Day 13, Part 1: " + new Day13().SolvePart1());
            Console.WriteLine("Day 13, Part 2: " + new Day13().SolvePart2());

            Console.ReadKey();
        }
    }
}
