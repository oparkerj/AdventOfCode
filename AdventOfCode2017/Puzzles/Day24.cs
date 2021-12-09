using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2017.Puzzles;

public class Day24 : Puzzle
{
    public Day24()
    {
        Part = 2;
    }

    public List<Component> GetComponents()
    {
        return Input.Extract<Component>(@"(\d+)/(\d+)").ToList();
    }

    public int Search(int last, List<Component> available)
    {
        var max = 0;
        var chosen = 0;
        foreach (var c in available.Where(c => c.A == last || c.B == last))
        {
            var other = c.A == last ? c.B : c.A;
            var remaining = new List<Component>(available.Without(c));
            var path = Search(other, remaining);
            if (max == 0 || path > max)
            {
                max = path;
                chosen = c.A + c.B;
            }
        }
        return max + chosen;
    }

    public override void PartOne()
    {
        var result = Search(0, GetComponents());
        WriteLn(result);
    }

    public (int Length, int Strength) SearchLongest(int last, List<Component> available, int length = 0)
    {
        var maxLen = 0;
        var maxStrength = 0;
        var chosen = 0;
        foreach (var c in available.Where(c => c.A == last || c.B == last))
        {
            var other = c.A == last ? c.B : c.A;
            var remaining = new List<Component>(available.Without(c));
            var (len, strength) = SearchLongest(other, remaining, length + 1);
            if (maxLen == 0 || len > maxLen || (len == maxLen && strength > maxStrength))
            {
                maxLen = len;
                maxStrength = strength;
                chosen = c.A + c.B;
            }
        }
        return (maxLen + length, maxStrength + chosen);
    }

    public override void PartTwo()
    {
        var (_, strength) = SearchLongest(0, GetComponents());
        WriteLn(strength);
    }

    public record Component(int A, int B);
}