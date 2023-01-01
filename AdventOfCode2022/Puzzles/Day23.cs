using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day23 : Puzzle<int>
{
    public const char Empty = '.';
    public const char Elf = '#';

    public override int PartOne()
    {
        var map = Input.ToGrid();
        map.Default = Empty;
        var temp = new Grid<char>();
        temp.Default = Empty;
        var proposed = new Dictionary<Pos, Pos>();
        var noMove = new HashSet<Pos>();

        var search = new CircularBuffer<Pos>(new[] {Pos.Up, Pos.Down, Pos.Left, Pos.Right});

        int Round()
        {
            temp.Clear();
            proposed.Clear();
            noMove.Clear();
            foreach (var pos in map.WhereValue(Elf).Keys())
            {
                var m = map;
                if (pos.Around().All(p => m[p] != Elf))
                {
                    temp[pos] = Elf;
                    continue;
                }
                Pos? moveTo = null;
                for (var i = 0; i < search.Count; i++)
                {
                    var dir = search[i];
                    if (map[pos + dir] == Empty && map[pos + dir + dir.Clockwise()] == Empty && map[pos + dir + dir.CounterClockwise()] == Empty)
                    {
                        moveTo = pos + dir;
                        break;
                    }
                }
                if (moveTo == null)
                {
                    temp[pos] = Elf;
                    continue;
                }
                if (proposed.TryGetValue(moveTo.Value, out var original))
                {
                    temp[pos] = Elf;
                    noMove.Add(original);
                }
                else
                {
                    proposed[moveTo.Value] = pos;
                }
            }

            var moved = 0;
            foreach (var (target, original) in proposed)
            {
                if (noMove.Contains(original)) temp[original] = Elf;
                else
                {
                    temp[target] = Elf;
                    moved++;
                }
            }
            
            Data.Swap(ref map, ref temp);
            search.Offset++;
            return moved;
        }

        if (Part == 1)
        {
            10.Times(() => Round());
            map.ResetBounds();
            return map.Bounds.Area - map.CountValues(Elf);
        }
        
        var rounds = 1;
        while (Round() != 0) rounds++;
        return rounds;
    }
}