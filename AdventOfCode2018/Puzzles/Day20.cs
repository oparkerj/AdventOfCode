using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles
{
    public class Day20 : Puzzle
    {
        // Grid is the minimum number of doors to get to each location
        public Grid<int> Map;

        public Day20()
        {
            Map = new Grid<int>();
            ExploreFrom(Pos.Origin, 0, null, InputLine[1..^1]);
            
            Part = 2;
        }

        public Pos GetDirection(char c)
        {
            return c switch
            {
                'N' => Pos.Up,
                'E' => Pos.Right,
                'S' => Pos.Down,
                'W' => Pos.Left,
                _ => throw new Exception("Invalid char")
            };
        }

        public void ExploreFrom(Pos start, int dist, string current, string rest)
        {
            // Loop created from tail-recursion
            while (true)
            {
                if (string.IsNullOrEmpty(current))
                {
                    if (string.IsNullOrEmpty(rest)) return;
                    if (rest.FindFirstParens(out var open, out var close))
                    {
                        if (open == 0)
                        {
                            current = rest[..(close + 1)];
                            rest = rest[(close + 1)..];
                            continue;
                        }
                        else
                        {
                            current = rest[..open];
                            rest = rest[open..];
                            continue;
                        }
                    }
                    else
                    {
                        current = rest;
                        rest = null;
                        continue;
                    }
                }
                else
                {
                    Map.SetDefault(start, dist);
                    if (current.StartsWith('('))
                    {
                        foreach (var part in current[1..^1].SplitOuter('|'))
                        {
                            ExploreFrom(start, dist, null, part + rest);
                        }
                    }
                    else
                    {
                        foreach (var dir in current.Select(GetDirection))
                        {
                            start += dir;
                            dist = Map.SetDefault(start, dist + 1);
                        }
                        current = null;
                        continue;
                    }
                }
                break;
            }
        }

        public override void PartOne()
        {
            WriteLn(Map.Values.Max());
        }

        public override void PartTwo()
        {
            WriteLn(Map.Values.Count(i => i >= 1000));
        }
    }
}