using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Solvers;
using MoreLinq;

namespace AdventOfCode2021.Puzzles;

public class Day8 : Puzzle
{
    public Day8()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var result = Input.Select(s => s.SingleSplit(" | ").Right
            .Spaced()
            .Select(pattern => pattern.Length)
            .Count(i => i is 2 or 3 or 4 or 7)).Sum();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var total = 0;

        var expected = new Dictionary<char, int>
        {
            ['a'] = 8,
            ['b'] = 6,
            ['c'] = 8,
            ['d'] = 7,
            ['e'] = 4,
            ['f'] = 9,
            ['g'] = 7,
        };

        foreach (var entry in Input)
        {
            var possible = new OneToOne<char, char>();
            possible.AddValues("abcdefg");
            possible.AddKeys("abcdefg");
                
            var (input, output) = entry.SingleSplit(" | ");
            var pattern = input.Spaced().ToList();
            var result = output.Spaced().ToList();
                
            foreach (var s in pattern)
            {
                if (s.Length == 2) possible.MustBe(s, "cf");
                else if (s.Length == 3) possible.MustBe(s, "acf");
                else if (s.Length == 4) possible.MustBe(s, "bdcf");
            }

            var actual = pattern.Flatten().Frequencies().ToDictionary();
            possible.ReduceWithFrequencies(expected, actual);
                
            var map = possible.Mappings();
            var value = result.Select(s => s.Select(c => map[c]).Sorted().Str())
                .Select(s => s switch
                {
                    "abcefg" => 0,
                    "cf" => 1,
                    "acdeg" => 2,
                    "acdfg" => 3,
                    "bcdf" => 4,
                    "abdfg" => 5,
                    "abdefg" => 6,
                    "acf" => 7,
                    "abcdefg" => 8,
                    "abcdfg" => 9,
                    _ => 0
                }).Str().AsInt();
            total += value;
        }
        WriteLn(total);
    }
}

public class Day8V2 : Puzzle
{
    public Day8V2()
    {
        Part = 2;
        InputName = "Day8.txt";
    }

    public override void PartOne() => new Day8().PartOne();

    public override void PartTwo()
    {
        var total = 0;
        foreach (var line in Input)
        {
            var (digits, pattern) = line.SingleSplit(" | ");
            var counts = digits.Spaced().ToDictionaryFirst(s => s.Length, s => s);
            var num = pattern.Spaced()
                .Select(s => (s.Length, s.Intersect(counts[4]).Count(), s.Intersect(counts[2]).Count()))
                .Select(tuple => tuple switch
                {
                    (2, _, _) => 1,
                    (3, _, _) => 7,
                    (4, _, _) => 4,
                    (7, _, _) => 8,
                    (5, 2, _) => 2,
                    (5, 3, 1) => 5,
                    (5, 3, 2) => 3,
                    (6, 4, _) => 9,
                    (6, 3, 1) => 6,
                    (6, 3, 2) => 0,
                    _ => 0
                }).Str().AsInt();
            total += num;
        }
        WriteLn(total);
    }
}