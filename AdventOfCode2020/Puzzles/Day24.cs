using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day24 : Puzzle
    {
        public DefaultDict<(int, int, int), bool> Tiles = new();
        public DefaultDict<(int, int, int), bool> Temp = new();
        public (int, int, int)[] Relative;

        public Day24()
        {
            Part = 2;
        }

        public (int, int, int) GetRelative(string spec)
        {
            return spec switch
            {
                "e" => (1, 0, -1),
                "w" => (-1, 0, 1),
                "ne" => (1, -1, 0),
                "sw" => (-1, 1, 0),
                "nw" => (0, -1, 1),
                "se" => (0, 1, -1),
                _ => throw new Exception()
            };
        }

        public IEnumerable<(int, int, int)> Around((int, int, int) p)
        {
            if (Relative == null) Relative = GetParts("ewnenwsesw").Select(GetRelative).ToArray();
            return Relative.Select(tuple => Add(tuple, p));
        }

        public (int, int, int) Add((int a, int b, int c) a, (int a, int b, int c) b)
        {
            return (a.a + b.a, a.b + b.b, a.c + b.c);
        }

        public IEnumerable<string> GetParts(string rel)
        {
            return rel.Extract<List<string>>("((?:n|s)?(?:e|w))+");
        }

        public override void PartOne()
        {
            foreach (var s in Input)
            {
                var at = GetParts(s).Select(GetRelative).Aggregate((0, 0, 0), Add);
                Tiles[at] = !Tiles[at];
            }
            WriteLn(Tiles.Count(pair => pair.Value));
        }

        public void Cycle()
        {
            var where = Tiles.Keys.SelectMany(Around).ToHashSet();
            foreach (var at in where)
            {
                var count = Around(at).Count(tuple => Tiles[tuple]);
                var flip = Tiles[at];
                if (flip && count is 0 or > 2) Temp[at] = false;
                else if (!flip && count == 2) Temp[at] = true;
                else Temp[at] = flip;
            }
            Data.Swap(ref Tiles, ref Temp);
            Temp.Clear();
        }

        public override void PartTwo()
        {
            PartOne();
            100.Times(Cycle);
            WriteLn(Tiles.Count(pair => pair.Value));
        }
    }
}