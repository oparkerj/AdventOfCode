using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day14 : Puzzle<int>
{
    public const int Air = 0;
    public const int Rock = 1;
    public const int Sand = 2;

    public override int PartOne()
    {
        var map = new Grid<int>();
        
        foreach (var line in Input)
        {
            var rocks = line.Split(" -> ").Select(Pos.Parse).ConnectLinesAll();
            foreach (var rock in rocks)
            {
                map[rock] = Rock;
            }
        }

        var source = new Pos(500, 0);

        bool Produce()
        {
            var current = source;
            while (true)
            {
                if (current.Y > map.Bounds.MaxY) return false;
                if (map[current + Pos.Up] == Air) current += Pos.Up;
                else if (map[current + Pos.Up + Pos.Left] == Air) current += Pos.Up + Pos.Left;
                else if (map[current + Pos.Up + Pos.Right] == Air) current += Pos.Up + Pos.Right;
                else
                {
                    map[current] = Sand;
                    return true;
                }
            }
        }

        var count = 0;
        while (Produce())
        {
            count++;
        }
        return count;
    }

    public override int PartTwo()
    {
        var used = new HashSet<Pos>();
        var max = 0;

        foreach (var line in Input)
        {
            var points = line.Split(" -> ").Select(Pos.Parse).ToList();
            var rocks = points.ConnectLines().Prepend(points[0]);
            foreach (var rock in rocks)
            {
                used.Add(rock);
                max = Math.Max(rock.Y, max);
            }
        }

        var source = new Pos(500, 0);

        void Produce()
        {
            var current = source;
            while (true)
            {
                if (current.Y >= max + 1) break;
                if (!used.Contains(current + Pos.Up)) current += Pos.Up;
                else if (!used.Contains(current + Pos.Up + Pos.Left)) current += Pos.Up + Pos.Left;
                else if (!used.Contains(current + Pos.Up + Pos.Right)) current += Pos.Up + Pos.Right;
                else break;
            }
            used.Add(current);
        }

        var count = 0;
        while (!used.Contains(source))
        {
            Produce();
            count++;
        }
        return count;
    }
}