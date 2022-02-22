using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day6 : Puzzle
{
    public Day6()
    {
        Part = 2;
    }

    public char MostCommonLetter(string s)
    {
        return s.Frequencies().MaxBy(pair => pair.Value).Key;
    }

    public override void PartOne()
    {
        var code = Input.Transpose().StrEach().Select(MostCommonLetter).Str();
        WriteLn(code);
    }
    
    public char LeastCommonLetter(string s)
    {
        return s.Frequencies().MinBy(pair => pair.Value).Key;
    }

    public override void PartTwo()
    {
        var code = Input.Transpose().StrEach().Select(LeastCommonLetter).Str();
        WriteLn(code);
    }
}