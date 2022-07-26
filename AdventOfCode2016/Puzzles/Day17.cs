using System.Security.Cryptography;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2016.Puzzles;

public class Day17 : Puzzle
{
    public MD5 Md5;
    public string Passcode;
    
    public Pos Target;
    public Rect Area;

    public Day17()
    {
        Passcode = InputLine;
        Target = (3, -3);
        Area = new Rect((0, 0), (3, -3));
        Md5 = MD5.Create();
    }

    public IEnumerable<PathRoom> Neighbors(PathRoom room)
    {
        if (room.Pos == Target) return Enumerable.Empty<PathRoom>();
        var hash = (Passcode + room.Path).Hash(Md5);
        return "UDLR".Zip(hash.Take(4))
            .Where(tuple => tuple.Second >= 'B')
            .TupleFirst()
            .Select(room.Relative);
    }

    public override void PartOne()
    {
        var dijkstra = new Dijkstra<PathRoom>(Neighbors);
        var start = new PathRoom(Pos.Zero);
        
        using (Md5)
        {
            var (_, dest) = dijkstra.ComputeFind(start, room => room.Pos == Target, room => Area.Contains(room.Pos));
            WriteLn(dest.Path);
        }
    }

    public override void PartTwo()
    {
        var dijkstra = new Dijkstra<PathRoom>(Neighbors);
        var start = new PathRoom(Pos.Zero);

        using (Md5)
        {
            var found = dijkstra.Compute(start, room => Area.Contains(room.Pos));
            var result = found.WhereKey(room => room.Pos == Target).Values().Max();
            WriteLn(result);
        }
    }
}