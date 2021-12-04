using System.Collections.Generic;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day20 : Puzzle
    {
        public const char Open = '.';

        public Day20()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var map = Input.ToGrid<char, PortalGrid<char>>();
            var portals = new Dictionary<string, (Side Side, Pos Pos)>();

            void AddPortal(string key, Side side, Pos pos)
            {
                if (portals.TryGetValue(key, out var portal))
                {
                    map.Connect(pos, side, portal.Pos, portal.Side);
                }
                else portals[key] = (side, pos);
            }
            
            // Find portals
            foreach (var (pos, c) in map)
            {
                if (!char.IsLetter(c)) continue;
                if (map[pos + Pos.Right] is var r && char.IsLetter(r))
                {
                    var side = map[pos + Pos.Left] == Open ? Side.Right : Side.Left;
                    var spot = side == Side.Right ? pos + Pos.Left : pos + Pos.Right * 2;
                    var key = $"{c}{r}";
                    AddPortal(key, side, spot);
                }
                else if (map[pos + Pos.Down] is var d && char.IsLetter(d))
                {
                    var side = map[pos + Pos.Up] == Open ? Side.Bottom : Side.Top;
                    var spot = side == Side.Bottom ? pos + Pos.Up : pos + Pos.Down * 2;
                    var key = $"{c}{d}";
                    AddPortal(key, side, spot);
                }
            }

            var start = portals["AA"].Pos;
            var target = portals["ZZ"].Pos;
            var dist = map.DijkstraFind(start, target, pos => map[pos] == Open);
            WriteLn(dist);
        }

        public override void PartTwo()
        {
            var map = new PortalGrid<char, int>();
            Input.ToGrid().ForEach(pair => map[pair.Key] = pair.Value);
            var mid = new Pos(InputLine.Length / 2, -Input.Length / 2);
            var portals = new Dictionary<string, (Side Side, Pos Pos, int Tag)>();

            void AddPortal(string key, Side side, Pos pos, int tag)
            {
                if (portals.TryGetValue(key, out var portal))
                {
                    map.Connect(pos, side, tag, portal.Pos, portal.Side, portal.Tag);
                }
                else portals[key] = (side, pos, tag);
            }
            
            // Find portals
            foreach (var ((pos, _), c) in map)
            {
                if (!char.IsLetter(c)) continue;
                if (map[pos + Pos.Right] is var r && char.IsLetter(r))
                {
                    var side = map[pos + Pos.Left] == Open ? Side.Right : Side.Left;
                    var spot = side == Side.Right ? pos + Pos.Left : pos + Pos.Right * 2;
                    var key = $"{c}{r}";
                    var inside = (spot - pos).Dot(pos - mid) > 0;
                    AddPortal(key, side, spot, inside ? 1 : -1);
                }
                else if (map[pos + Pos.Down] is var d && char.IsLetter(d))
                {
                    var side = map[pos + Pos.Up] == Open ? Side.Bottom : Side.Top;
                    var spot = side == Side.Bottom ? pos + Pos.Up : pos + Pos.Down * 2;
                    var key = $"{c}{d}";
                    var inside = (spot - pos).Dot(pos - mid) > 0;
                    AddPortal(key, side, spot, inside ? 1 : -1);
                }
            }

            // tag = what level we are in
            // Block passage to outer level if we are at level 0.
            map.PassFunction = ((Pos Pos, int Tag) current, Pos _, ref int tag) =>
            {
                var toOuter = tag == -1;
                tag += current.Tag;
                if (toOuter) return current.Tag > 0;
                return true;
            };

            var start = portals["AA"].Pos;
            var target = portals["ZZ"].Pos;
            var dist = map.DijkstraFind((start, 0), (target, 0), tuple => map[tuple.Item1] == Open);
            WriteLn(dist);
        }
    }
}