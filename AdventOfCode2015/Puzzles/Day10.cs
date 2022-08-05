using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day10 : Puzzle<int>
{
    public string Next(string s) => s.RunLengthEncode().Select(pair => $"{pair.Value}{pair.Key}").Str();

    public override int PartOne() => InputLine.Repeat(Next, Part == 2 ? 50 : 40).Length;
}