using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2018.Puzzles;

public class Day22 : Puzzle
{
    public const int Rocky = 0;
    public const int Wet = 1;
    public const int Narrow = 2;

    public const int Neither = 0;
    public const int Torch = 1;
    public const int ClimbingGear = 2;

    public int Depth;
    public Pos Target;

    public Day22()
    {
        var info = Input.ReadKeys();
        Depth = info["depth"].AsInt();
        Target = Pos.Parse(info["target"]);
        Part = 2;
    }

    public IEnumerable<int> Tools(int type)
    {
        if (type is Rocky or Wet) yield return ClimbingGear;
        if (type is Rocky or Narrow) yield return Torch;
        if (type is Wet or Narrow) yield return Neither;
    }

    public FixedGrid<(int geo, int erosion)> BuildMap()
    {
        var target = Target;
        var cave = new ExpandingFixedGrid<(int geo, int erosion)>(target.X + 1, target.Y + 1);
        foreach (var pos in cave.Bounds)
        {
            int geo;
            if (pos == Pos.Origin) geo = 0;
            else if (pos == target) geo = 0;
            else if (pos.Y == 0) geo = pos.X * 16807;
            else if (pos.X == 0) geo = pos.Y * 48271;
            else geo = cave[pos + Pos.Left].erosion * cave[pos + Pos.Down].erosion;
            cave[pos] = Create(geo);
        }
        return cave;
    }

    public (int geo, int erosion) Create(int geo) => (geo, (geo + Depth) % 20183);

    // Get the value assuming the left and down exist
    public (int geo, int erosion) GetKnownValue(FixedGrid<(int geo, int erosion)> cave, Pos pos)
    {
        var geo = cave[pos + Pos.Left].erosion * cave[pos + Pos.Down].erosion;
        return Create(geo);
    }

    // Get the value by computing all intermediate values needed
    public (int geo, int erosion) GetValue(FixedGrid<(int geo, int erosion)> cave, Pos pos)
    {
        if (cave.TryGet(pos, out var result)) return result;
        if (cave.Has(pos + Pos.Left) && cave.Has(pos + Pos.Down))
        {
            return GetKnownValue(cave, pos);
        }
        var compute = new Stack<Pos>();
        var c = new HashSet<Pos>(); // c == compute but for fast lookup
        var add = new Queue<Pos>();
        add.Enqueue(pos);
        while (add.Count > 0)
        {
            var next = add.Dequeue();
            if (c.Contains(next) || cave.Has(next)) continue;
            if (next.Y == 0)
            {
                cave[next] = Create(next.X * 16807);
                continue;
            }
            if (next.X == 0)
            {
                cave[next] = Create(next.Y * 48271);
                continue;
            }
            compute.Push(next);
            c.Add(next);
            add.Enqueue(next + Pos.Left);
            add.Enqueue(next + Pos.Down);
        }
        while (compute.Count > 0)
        {
            var next = compute.Pop();
            cave[next] = GetKnownValue(cave, next);
        }
        return cave[pos];
    }

    public int Type((int geo, int erosion) data) => data.erosion % 3;

    public override void PartOne()
    {
        var cave = BuildMap();
        var result = cave.Values.Sum(Type);
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var cave = BuildMap();

        IEnumerable<Region> Possible(Region r)
        {
            var type = Type(GetValue(cave, r.Position));
            var tools = Tools(type).ToArray(2);
            // Adjacent spaces you can go to
            return r.Position.Adjacent()
                .Where(pos => pos.NonNegative)
                .Where(pos => Tools(Type(GetValue(cave, pos))).Contains(r.Tool))
                .Select(pos => new Region(pos, r.Tool))
                // Or changing tools in the same spot
                .Concat(tools.Without(r.Tool).Select(i => new Region(r.Position, i, true)));
        }

        var dijkstra = new Dijkstra<Region, Region>
        {
            Neighbors = Possible,
            Cell = (_, b) => b,
            Distance = r => r.ToolChange ? 7 : 1
        };

        var start = new Region(Pos.Origin, Torch);
        var target = new Region(Target, Torch);
        var time = dijkstra.ComputeFind(start, target);
        WriteLn(time);
    }

    public readonly struct Region
    {
        public readonly Pos Position;
        public readonly int Tool;
        public readonly bool ToolChange;

        public Region(Pos position, int tool, bool toolChange = false)
        {
            Position = position;
            Tool = tool;
            ToolChange = toolChange;
        }

        public bool Equals(Region other) => Position.Equals(other.Position) && Tool == other.Tool;

        public override bool Equals(object obj) => obj is Region other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Position, Tool);
    }
}