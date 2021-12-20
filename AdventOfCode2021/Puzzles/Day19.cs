using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;

namespace AdventOfCode2021.Puzzles;

public class Day19 : Puzzle
{
    public Day19()
    {
        Part = 2;
    }

    public Cloud FreshCloud(int index)
    {
        return new Cloud(AllGroups[index].Skip(1).Select(Pos3D.Parse));
    }

    public bool TryAlign(Cloud main, Cloud align)
    {
        foreach (var anchor in main)
        {
            foreach (var pos in align.ToList())
            {
                var shift = anchor - pos;
                align.Shift(shift);
                if (main.CountOverlap(align) >= 12) return true;
                align.Shift(-shift);
            }
        }
        return false;
    }

    public bool FullAlign(Cloud main, Cloud scanners, int i)
    {
        var cloud = FreshCloud(i);
        var found = cloud.TryAllOrientations(c => TryAlign(main, c));
        if (!found) return false;
        main.AddAll(cloud);
        scanners.Add(cloud.Offset);
        return true;
    }

    public (Cloud Beacons, Cloud Scanners) BuildMap()
    {
        var main = new Cloud(FreshCloud(0));
        var scanners = new Cloud {Pos3D.Origin};

        var aligned = 1;
        var remaining = new HashSet<int>(new Interval(1, AllGroups.Length - 1));
        while (aligned < AllGroups.Length)
        {
            foreach (var i in remaining.ToList().Where(i => FullAlign(main, scanners, i)))
            {
                aligned++;
                remaining.Remove(i);
            }
        }
        return (main, scanners);
    }

    public override void PartOne()
    {
        var (beacons, _) = BuildMap();
        WriteLn(beacons.Count);
    }

    public override void PartTwo()
    {
        var (_, scanners) = BuildMap();
        var dist = scanners.ToList().Pairs().Max(tuple => tuple.Item1.MDist(tuple.Item2));
        WriteLn(dist);
    }
}