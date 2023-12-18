using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

internal class Day05 : Solution
{
    static List<string> lines;
    static Almanac almanac;
    static Day05()
    {
        lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, $"..\\..\\..\\{nameof(Day05)}\\{nameof(Day05)}.txt")).ToList();
        almanac = new Almanac(
        GetMappingThatStartsWith("seed-to-soil"),
        GetMappingThatStartsWith("soil-to-fertilizer"),
        GetMappingThatStartsWith("fertilizer-to-water"),
        GetMappingThatStartsWith("water-to-light"),
        GetMappingThatStartsWith("light-to-temperature"),
        GetMappingThatStartsWith("temperature-to-humidity"),
        GetMappingThatStartsWith("humidity-to-location"));
    }

    public override string SolvePart1()
    {
        var seeds = lines[0].Split("seeds:")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
        return seeds.Select(s => almanac.GetLocation(s)).Min().ToString();
    }

    public override string SolvePart2()
    {
        List<SeedRange> ranges = new List<SeedRange>();
        var seedRangePairs = lines[0].Split("seeds:")[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < seedRangePairs.Length; i += 2)
        {
            var min = long.Parse(seedRangePairs[i]);
            var range = long.Parse(seedRangePairs[i + 1]) - 1;
            ranges.Add(new SeedRange(min, min + range));
        }
        // long minSeedNumber = almanac.GetMinimumMappedLocation();

        long locationGuess = 0;
        while (!SeedRangesContain(ranges, almanac.GetMappedLocation(locationGuess)))
            locationGuess+=1000;
        locationGuess -= 999;
        while (!SeedRangesContain(ranges, almanac.GetMappedLocation(locationGuess)))
            locationGuess += 1;


        return locationGuess.ToString();
    }

    bool SeedRangesContain(List<SeedRange> ranges, long seedNumber)
    {
        return ranges.Any(range => range.minValue <= seedNumber && range.maxValue >= seedNumber);
    }

    class SeedRange
    {
        public long minValue;
        public long maxValue;
        public SeedRange(long minValue, long maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public bool ContainsValue(long v) => v >= minValue && v <= maxValue;


        public override string ToString()
        {
            return $"{minValue} - {maxValue}";
        }
    }

    static MappingSet GetMappingThatStartsWith(string title)
    {
        MappingSet newSet = new MappingSet();

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            if (line.StartsWith(title))
            {
                int idx = 1;

                while (true)
                {
                    if (i + idx >= lines.Count)
                    {
                        break;
                    }
                    line = lines[i + idx++];
                    if (string.IsNullOrEmpty(line))
                        break;
                    newSet.AddMapping(line);
                }
            }
        }
        return newSet;
    }

    class Almanac
    {
        List<MappingSet> mappingSets;

        public Almanac(params MappingSet[] mappingSets)
        {
            this.mappingSets = mappingSets.ToList();
        }

        public long GetLocation(long seedValue)
        {
            long currentValue = seedValue;
            foreach (var mappingSet in mappingSets)
            {
                currentValue = mappingSet.GetMappedValue(currentValue);
            }
            return currentValue;
        }

        public List<SeedRange> GetLocationRanges(SeedRange range)
        {
            Stack<SeedRange> rangeStack = new Stack<SeedRange>();
            rangeStack.Push(range);

            List<SeedRange> unmappedSet = new List<SeedRange>();
            List<SeedRange> mappedSet = new List<SeedRange>();
            foreach (var mappingSet in mappingSets)
            {
                unmappedSet.Clear();
                mappedSet.Clear();
                while (rangeStack.Count > 0)
                {
                    var unmappedRange = rangeStack.Pop();
                    var newUnmappedRanges = mappingSet.GetMappedRanges(unmappedRange, out var mappedRange);
                    mappedSet.AddRange(mappedRange);
                    unmappedSet.AddRange(newUnmappedRanges);
                    ConsolidateRanges(unmappedSet);
                    ConsolidateRanges(mappedRange);
                }
                unmappedSet.AddRange(mappedSet);
                ConsolidateRanges(unmappedSet);
                unmappedSet.ForEach(rangeStack.Push);
            }
            return rangeStack.ToList();
        }

        public long GetMinimumMappedLocation()
        {
            long targetPosition = -1;
            for (int i = mappingSets.Count - 1; i >= 0; i--)
            {
                var set = mappingSets[i];
                if (targetPosition < 0)
                    targetPosition = set.GetMinimumDestination();
                else
                    targetPosition = set.GetValueThatMapsTo(targetPosition);
            }
            return targetPosition;
        }

        public long GetMappedLocation(long destination)
        {
            long targetPosition = destination;
            for (int i = mappingSets.Count - 1; i >= 0; i--)
            {
                var set = mappingSets[i];
                targetPosition = set.GetValueThatMapsTo(targetPosition);
            }
            return targetPosition;
        }
    }

    class MappingSet
    {
        List<Mapping> mappings = new List<Mapping>();
        public void AddMapping(string mapString)
        {
            var values = mapString.Split(' ');
            mappings.Add(new Mapping(long.Parse(values[0]), long.Parse(values[1]), long.Parse(values[2])));
        }

        public long GetMappedValue(long sourceValue)
        {
            foreach (var mapping in mappings)
            {
                long destinationValue = mapping.GetMappedValue(sourceValue);
                if (destinationValue != -1)
                    return destinationValue;
            }
            return sourceValue;
        }
        public List<SeedRange> GetMappedRanges(SeedRange range, out List<SeedRange> mappedRanges)
        {
            mappedRanges = new List<SeedRange>();
            List<SeedRange> unmappedRanges = new List<SeedRange>() { range };
            //List<SeedRange> newUnmappedRanges = new List<SeedRange>();
            //foreach (var mapping in mappings)
            //{
            //    newUnmappedRanges.Clear();

            //    foreach (var unmappedRange in unmappedRanges)
            //    {
            //        newUnmappedRanges.AddRange(mapping.GetMappedRanges(unmappedRange, out SeedRange mappedRange));
            //        if (mappedRange != null)
            //        {
            //            mappedRanges.Add(mappedRange);

            //            ConsolidateRanges(mappedRanges);
            //        }
            //        ConsolidateRanges(newUnmappedRanges);
            //    }
            //    unmappedRanges.Clear();
            //    unmappedRanges.AddRange(newUnmappedRanges);
            //}
            return unmappedRanges;
        }


        public long GetValueThatMapsTo(long destination)
        {
            foreach (var mapping in mappings)
            {
                var targetSource = mapping.GetValueThatMapsTo(destination);
                if (targetSource != destination)
                {
                    return targetSource;
                }
            }
            return destination;

        }

        public long GetMinimumDestination() => mappings.Min(map => map.destMin);
    }

    class Mapping
    {
        public readonly long destMin;
        readonly long destMax;
        readonly long sourceMax;
        readonly long sourceMin;
        readonly long length;
        readonly long delta;

        public Mapping(long destinationStart, long sourceStart, long length)
        {
            this.destMin = destinationStart;
            this.sourceMin = sourceStart;
            this.length = length;
            destMax = destinationStart + length;
            sourceMax = sourceStart + length;
            delta = destinationStart - sourceStart;
        }

        public long GetMappedValue(long sourceValue)
        {
            if (sourceValue < sourceMin || sourceValue > sourceMin + length)
                return -1;
            return destMin + sourceValue - sourceMin;
        }


        public long GetValueThatMapsTo(long destination)
        {
            if (destination >= destMin && destination <= destMax)
                return destination - delta;
            return destination;
        }

        public override string ToString()
        {
            return $"{sourceMin}-{sourceMin + length - 1} -> {destMin}->{destMin + length - 1}";
        }
    }

    static void ConsolidateRanges(List<SeedRange> ranges)
    {
        bool changed;
        do
        {
            changed = false;
            for (int i = 0; i < ranges.Count; i++)
            {
                for (int j = i + 1; j < ranges.Count; j++)
                {
                    var rangeA = ranges[i];
                    var rangeB = ranges[j];

                    if (rangeA.ContainsValue(rangeB.minValue) ||
                        rangeA.ContainsValue(rangeB.maxValue) ||
                        rangeB.ContainsValue(rangeA.minValue) ||
                        rangeB.ContainsValue(rangeA.maxValue))

                    {
                        changed = true;
                        ranges.RemoveAt(j);
                        rangeA.minValue = Math.Min(rangeA.minValue, rangeB.minValue);
                        rangeA.maxValue = Math.Max(rangeA.maxValue, rangeB.maxValue);
                        j--;
                    }
                }
            }
        }
        while (changed);
    }
}

