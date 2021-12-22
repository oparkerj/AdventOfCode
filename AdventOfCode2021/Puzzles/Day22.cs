using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using RegExtract;

namespace AdventOfCode2021.Puzzles;

public class Day22 : Puzzle
{
    public Day22()
    {
        Part = 2;
    }
    
    public record Region(Interval X, Interval Y, Interval Z, bool On = false);

    public Region Read(string line)
    {
        var (a, b, c, d, e, f) = line.Extract<(int, int, int, int, int, int)>(@".+ x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)");
        return new Region(Interval.RangeInclusive(a, b), Interval.RangeInclusive(c, d), Interval.RangeInclusive(e, f), line.StartsWith("on"));
    }

    public override void PartOne()
    {
        var blocks = new Blocks<bool>();
        var valid = Interval.RangeInclusive(-50, 50);
        
        foreach (var s in Input)
        {
            var (x, y, z, on) = Read(s);
            foreach (var i in x.Overlap(valid))
            {
                foreach (var j in y.Overlap(valid))
                {
                    foreach (var k in z.Overlap(valid))
                    {
                        blocks[new Pos3D(i, j, k)] = on;
                    }
                }
            }
        }
        
        WriteLn(blocks.CountValues(true));
    }

    public Region GetOverlap(Region first, Region second)
    {
        var (a, b, c, _) = first;
        var (d, e, f, _) = second;
        return new Region(a.Overlap(d), b.Overlap(e), c.Overlap(f));
    }

    public BigInteger Size(Region region)
    {
        var (x, y, z, _) = region;
        return (BigInteger) x.Length * y.Length * z.Length;
    }

    public BigInteger GetContribution(Region region, IEnumerable<Region> possibleOverlaps)
    {
        var volume = Size(region);
        var overlaps = new List<Region>();
        foreach (var next in possibleOverlaps)
        {
            var (ox, oy, oz, _) = GetOverlap(region, next);
            if (ox.Length == 0 || oy.Length == 0 || oz.Length == 0) continue;
            overlaps.Add(new Region(ox, oy, oz));
        }
        foreach (var (i, overlap) in overlaps.Indexed())
        {
            volume -= GetContribution(overlap, overlaps.Skip(i + 1));
        }
        return volume;
    }

    public override void PartTwo()
    {
        var regions = Input.Select(Read).ToList();
        var total = regions.Indexed()
            .WhereValue(r => r.On)
            .Select(pair => GetContribution(pair.Value, regions.Skip(pair.Key + 1)))
            .Aggregate(BigInteger.Add);
        Clip(total);
    }
}