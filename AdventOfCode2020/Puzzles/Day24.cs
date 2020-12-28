using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day24 : Puzzle
    {
        public DefaultDict<(int, int, int), bool> Tiles = new();
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
        
        public IEnumerable<(int, int, int)> Around((int, int, int) p)
        {
            if (Relative == null) Relative = GetParts("ewnenwsesw").Select(GetRelative).ToArray();
            return Relative.Select(tuple => Add(tuple, p));
        }

        public override void PartTwo()
        {
            PartOne();
            var game = new GameOfLife<(int, int, int)>()
                .WithNeighborFunction(Around)
                .WithLivingDeadRules(i => i is 0 or > 2, i => i == 2)
                .WithExpansion()
                .WithKeepDead(false);
            foreach (var (pos, flip) in Tiles)
            {
                game[pos] = flip;
            }
            game.Step(100);
            WriteLn(game.CountActive());
        }
    }
}