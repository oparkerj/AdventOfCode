using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day11 : Puzzle
    {
        public Day11()
        {
            Part = 2;
        }

        public int Map(char c)
        {
            return c == 'L' ? 1 : 0;
        }

        public void ReadInput()
        {
            var height = Input.Length;
            var width = Input[0].Length;
            Seats = new int[width, height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    Seats[x, y] = Map(Input[y][x]);
                }
            }
        }

        public int Step()
        {
            var c = 0;
            var next = (int[,]) Seats.Clone();
            for (var i = 0; i < Seats.GetLength(0); i++)
            {
                for (var j = 0; j < Seats.GetLength(1); j++)
                {
                    var count = (i, j).Around().Count(p => Seats.GetOrDefault(p) == 2);
                    if (Seats[i, j] == 1 && count == 0)
                    {
                        next[i, j] = 2;
                        c++;
                    }
                    else if (Seats[i, j] == 2 && count >= 4)
                    {
                        next[i, j] = 1;
                        c++;
                    }
                }
            }
            Seats = next;
            return c;
        }

        public int[,] Seats;

        public override void PartOne()
        {
            ReadInput();
            while (true)
            {
                if (Step() == 0) break;
            }
            WriteLn(Seats.All().Count(i => i == 2));
        }

        public int Trace((int x, int y) p, (int x, int y) dir)
        {
            var (x, y) = p;
            var (dx, dy) = dir;
            while (true)
            {
                x += dx;
                y += dy;
                if (!Seats.Has((x, y))) return 0;
                if (Seats[x, y] > 0) return Seats[x, y];
            }
        }

        public int Step2()
        {
            var dirs = (0, 0).Around().ToArray();
            var c = 0;
            var next = (int[,]) Seats.Clone();
            for (var i = 0; i < Seats.GetLength(0); i++)
            {
                for (var j = 0; j < Seats.GetLength(1); j++)
                {
                    var count = dirs.Count(d => Trace((i, j), d) == 2);
                    if (Seats[i, j] == 1 && count == 0)
                    {
                        next[i, j] = 2;
                        c++;
                    }
                    else if (Seats[i, j] == 2 && count >= 5)
                    {
                        next[i, j] = 1;
                        c++;
                    }
                }
            }
            Seats = next;
            return c;
        }

        public override void PartTwo()
        {
            ReadInput();
            while (true)
            {
                if (Step2() == 0) break;
            }
            WriteLn(Seats.All().Count(i => i == 2));
        }
    }
}