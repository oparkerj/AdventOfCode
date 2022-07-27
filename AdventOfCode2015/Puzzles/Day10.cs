using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day10 : Puzzle
{
    public string Next(string s)
    {
        return s.RunLengthEncode().Select(pair => $"{pair.Value}{pair.Key}").Str();
    }
    
    public override void PartOne()
    {
        var current = InputLine;
        current = current.Repeat(Next, 40);
        WriteLn(current.Length);
    }

    public override void PartTwo()
    {
        var current = InputLine;
        current = current.Repeat(Next, 50);
        WriteLn(current.Length);
    }
}