using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day10 : Puzzle
{
    public string Next(string s) => s.RunLengthEncode().Select(pair => $"{pair.Value}{pair.Key}").Str();

    public override void PartOne() => WriteLn(InputLine.Repeat(Next, Part == 2 ? 50 : 40).Length);
}