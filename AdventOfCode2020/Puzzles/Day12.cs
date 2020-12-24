using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2020.Puzzles
{
    public class Day12 : Puzzle
    {
        public Day12()
        {
            Part = 2;
        }

        public static readonly Pos North = (0, 1);
        public static readonly Pos East = (1, 0);
        public static readonly Pos South = (0, -1);
        public static readonly Pos West = (-1, 0);

        public Pos[] Dirs = {North, East, South, West};
        
        public override void PartOne()
        {
            Pos at = (0, 0);
            var dir = 1;
            foreach (var s in Input)
            {
                var a = s[0];
                var v = int.Parse(s[1..]);
                if (a == 'N') at += North * v;
                else if (a == 'E') at += East * v;
                else if (a == 'S') at += South * v;
                else if (a == 'W') at += West * v;
                else if (a == 'L') dir = (dir + v / 90 * 3) % Dirs.Length;
                else if (a == 'R') dir = (dir + v / 90) % Dirs.Length;
                else if (a == 'F') at += Dirs[dir] * v;
            }
            WriteLn((0, 0).MDist(at));
        }

        public override void PartTwo()
        {
            Pos at = (0, 0);
            var point = East * 10 + North * 1;
            foreach (var s in Input)
            {
                var a = s[0];
                var v = int.Parse(s[1..]);
                if (a == 'N') point += North * v;
                else if (a == 'E') point += East * v;
                else if (a == 'S') point += South * v;
                else if (a == 'W') point += West * v;
                else if (a == 'L') point = point.Repeat(p => p.CounterClockwise(), v / 90);
                else if (a == 'R') point = point.Repeat(p => p.Clockwise(), v / 90);
                else if (a == 'F') at += point * v;
            }
            WriteLn((0, 0).MDist(at));
        }
    }
}