using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2018.Puzzles;

public class Day15 : Puzzle
{
    public const char Wall = '#';
    public const char Open = '.';
    public const char Friendly = 'E';

    public Grid<ITile> Map = new();

    public readonly Func<Pos, bool> Valid;

    public readonly IComparer<KeyValuePair<Pos, int>> BestMove =
        Comparing<KeyValuePair<Pos, int>>.Prefer(tuple => tuple.Value > 0)
            .ThenBy(tuple => tuple.Value)
            .ThenBy(tuple => tuple.Key, Pos.ReadingOrder);

    public readonly IComparer<Unit> AttackOrder =
        Comparing<Unit>.By(unit => unit.Health)
            .ThenBy(unit => unit.Position, Pos.ReadingOrder);

    public Day15()
    {
        Valid = pos => Map[pos] == null;
        foreach (var (pos, c) in Input.Positions().WhereValue(c => c != Open))
        {
            ITile tile = c == Wall ? new WallTile() : new Unit {Friendly = c == Friendly, Position = pos};
            Map[pos] = tile;
        }
        Part = 2;
    }

    public IEnumerable<Unit> Units()
    {
        return Map.Values.WhereType<Unit>();
    }

    public Func<ITile, bool> IsEnemy(Unit unit)
    {
        var f = unit.Friendly;
        return tile => tile is Unit u && u.Friendly != f;
    }

    public Unit TakeTurn(Unit unit, out bool combat)
    {
        List<Unit> GetEnemies(Unit u)
        {
            return u.Position.Adjacent().GetFrom(Map).Where(IsEnemy(u)).Cast<Unit>().ToList();
        }

        combat = true;
        // Move if not next to enemy
        var targets = Map.Values.Where(IsEnemy(unit)).Cast<Unit>().ToList();
        if (targets.Count == 0)
        {
            combat = false;
            return null;
        }
        if (targets.All(u => u.Position.MDist(unit.Position) > 1))
        {
            // Get target spots
            var inRange = Map.WhereValue(IsEnemy(unit))
                .Keys()
                .SelectMany(p => p.Adjacent())
                .Distinct()
                .Where(p => Map[p] == null)
                .ToList();
                
            // Select closest position to target
            var chosen = inRange.With(pos => Map.ShortestPathBfs(unit.Position, pos, Valid))
                .DefaultIfEmpty()
                .SelectMinBy(BestMove);
            if (chosen.Value <= 0) return null;
            // Move towards chosen spot
            var dist = Map.DijkstraWhere(chosen.Key, Valid);
            var move = unit.Position.Adjacent()
                .Where(dist.ContainsKey)
                .SelectMinBy(Comparing<Pos>.By(pos => dist[pos]).ThenBy(Pos.ReadingOrder));
            Map[unit.Position] = null;
            Map[move] = unit;
            unit.Position = move;
        }
        var adjacent = GetEnemies(unit);
        if (adjacent.Count == 0) return null;
        var attack = adjacent.SelectMinBy(AttackOrder);
        attack.Health -= unit.Power;
        if (attack.Health <= 0) return attack;
        return null;
    }

    public bool Round()
    {
        var units = Units()
            .OrderBy(unit => unit.Position, Pos.ReadingOrder)
            .ToList();
        for (var index = 0; index < units.Count; index++)
        {
            var unit = units[index];
            var killed = TakeTurn(unit, out var @continue);
            if (!@continue) return false;
            if (killed != null)
            {
                Map[killed.Position] = null;
                units.RemoveConcurrent(u => u == killed, ref index);
            }
        }
        return true;
    }

    public int SimulateBattle()
    {
        var count = 0;
        while (Round()) count++;
        var result = count * Units().Select(unit => unit.Health).Sum();
        return result;
    }

    public override void PartOne()
    {
        var result = SimulateBattle();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var initial = Map;
        var power = Units().First(unit => unit.Friendly).Power;
        var count = Units().Count(unit => unit.Friendly);
        int result;
        while (true)
        {
            // Reset map
            Map = new Grid<ITile>(initial);
            foreach (var (pos, tile) in Map)
            {
                if (tile is Unit unit)
                {
                    unit.Position = pos;
                    if (unit.Friendly) unit.Power = power;
                    unit.Health = 200;
                }
            }
            result = SimulateBattle();
            if (Units().Count(unit => unit.Friendly) == count) break;
            power++;
        }
        WriteLn(result);
    }

    public interface ITile { }

    public class WallTile : ITile
    {
        public override string ToString() => "#";
    }

    public class Unit : ITile
    {
        public Pos Position;
        public bool Friendly;
        public int Health = 200;
        public int Power = 3;

        public override string ToString() => Friendly ? "E" : "G";
    }
}