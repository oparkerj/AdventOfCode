using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2018.Puzzles;

public class Day3 : Puzzle
{
    public Day3()
    {
        Part = 2;
    }

    public IEnumerable<Rect> ReadClaims()
    {
        return Input.Extract<Rect>(@"#\d+ @ (?<MinX>\d+),(?<MinY>\d+): (?<Width>\d+)x(?<Height>\d+)");
    }

    public override void PartOne()
    {
        var claims = ReadClaims().ToList();
        var result = claims.Pairs()
            .SelectMany(pair => pair.Item1.Intersection(pair.Item2).Positions())
            .Distinct()
            .Count();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var claims = ReadClaims().ToList();
        var overlap = new HashSet<Rect>();
        foreach (var (a, b) in claims.Pairs().Where(tuple => tuple.Item1.Intersection(tuple.Item2).NonEmpty))
        {
            overlap.Add(a);
            overlap.Add(b);
        }
        var single = claims.Except(overlap).First();
        WriteLn(claims.IndexOf(single) + 1);
    }
}