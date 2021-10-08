using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day3 : Puzzle
    {
        public Day3()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var wires = Input.Select(s => new Wire(s)).ToArray();
            var a = wires[0];
            var b = wires[1];
            var min = a.Intersect(b)
                .Without(Pos.Origin)
                .Min(pos => pos.MDist(Pos.Origin));
            WriteLn(min);
        }

        public override void PartTwo()
        {
            var wires = Input.Select(s => new Wire(s)).ToArray();
            var a = wires[0];
            var b = wires[1];
            var ad = a.WithDistances().ToDictionary();
            var bd = b.WithDistances().ToDictionary();
            var min = a.Intersect(b)
                .Without(Pos.Origin)
                .Min(pos => ad[pos] + bd[pos]);
            WriteLn(min);
        }
    }

    public class Wire : IEnumerable<Pos>
    {
        public readonly List<Pos> Points = new();

        public Wire(string data)
        {
            var last = Pos.Origin;
            Points.Add(last);
            foreach (var section in data.Csv())
            {
                var next = last + Dir(section[0]) * int.Parse(section[1..]);
                Points.Add(last = next);
            }
        }

        private Pos Dir(char dir)
        {
            return dir switch
            {
                'U' => Pos.Up,
                'R' => Pos.Right,
                'L' => Pos.Left,
                'D' => Pos.Down,
                _ => Pos.Origin,
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Pos> GetEnumerator()
        {
            for (var i = 0; i < Points.Count; i++)
            {
                var point = Points[i];
                yield return point;
                if (i >= Points.Count - 1) continue;
                var next = Points[i + 1];
                var dir = (next - point).Normalize();
                while ((point += dir) != next) yield return point;
            }
        }

        public IEnumerable<KeyValuePair<Pos, int>> WithDistances()
        {
            return this.Select((pos, i) => new KeyValuePair<Pos, int>(pos, i));
        }
    }
}