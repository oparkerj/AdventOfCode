using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using RegExtract;

namespace AdventOfCode2020.Puzzles;

public class Day24 : Puzzle
{
    public DefaultDict<Pos3D, bool> Tiles = new();
    public Pos3D[] Relative;

    public Day24()
    {
        Part = 2;
    }

    public Pos3D GetRelative(string spec)
    {
        return spec switch
        {
            "e" => new Pos3D(1, 0, -1),
            "w" => new Pos3D(-1, 0, 1),
            "ne" => new Pos3D(1, -1, 0),
            "sw" => new Pos3D(-1, 1, 0),
            "nw" => new Pos3D(0, -1, 1),
            "se" => new Pos3D(0, 1, -1),
            _ => throw new Exception()
        };
    }

    public IEnumerable<string> GetParts(string rel)
    {
        return rel.Extract<List<string>>("((?:n|s)?(?:e|w))+");
    }

    public override void PartOne()
    {
        foreach (var s in Input)
        {
            var at = GetParts(s).Select(GetRelative).Aggregate(Pos3D.Origin, (a, b) => a + b);
            Tiles[at] = !Tiles[at];
        }
        WriteLn(Tiles.Count(pair => pair.Value));
    }
        
    public IEnumerable<Pos3D> Around(Pos3D p)
    {
        if (Relative == null) Relative = GetParts("ewnenwsesw").Select(GetRelative).ToArray();
        return Relative.Select(dir => p + dir);
    }

    public override void PartTwo()
    {
        PartOne();
        var game = new GameOfLife<Pos3D>()
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