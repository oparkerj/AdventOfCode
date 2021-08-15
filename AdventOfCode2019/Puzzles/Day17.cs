using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019.Puzzles
{
    public class Day17 : Puzzle
    {
        public const int Open = '.';
        public const int Scaffold = '#';

        public Grid<int> Map = new();

        public Day17()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var pos = Pos.Origin;
            var c = Computer.From(InputLine);
            c.LineOut = new OutputCondition()
                .CaseInt(i => i != '\n', i =>
                {
                    Map[pos] = i;
                    pos += Pos.Right;
                })
                .Else(_ => pos = new Pos(0, pos.Y - 1))
                .Line;
            c.Execute();
            var sum = Map.Where(pair => pair.Value == Scaffold && pair.Key.NeighborValues(Map).AllEqual(Scaffold))
                .Keys()
                .Select(p => p.X * -p.Y)
                .Sum();
            WriteLn(sum);
        }

        public record Move(bool Turn, int Dist)
        {
            public override string ToString() => $"{(Turn ? 'R' : 'L')},{Dist}";
        }

        public override void PartTwo()
        {
            Run(PartOne);
            // Get sequence of moves to traverse scaffold.
            var moves = new List<Move>();
            var pos = Map.Find('^');
            var dir = Pos.Up;
            while (true)
            {
                var left = dir.CounterClockwise();
                var right = dir.Clockwise();
                if (Map[pos + right] == Scaffold)
                {
                    var end = pos.Trace(right, p => Map[p + right] != Scaffold);
                    moves.Add(new Move(true, end.MDist(pos)));
                    pos = end;
                    dir = right;
                }
                else if (Map[pos + left] == Scaffold)
                {
                    var end = pos.Trace(left, p => Map[p + left] != Scaffold);
                    moves.Add(new Move(false, end.MDist(pos)));
                    pos = end;
                    dir = left;
                }
                else break;
            }
            
            // Find sections which are repeats
            const int maxLength = 20;
            var partial = new List<Move>(moves);
            var routines = new List<Move>[3];
            for (var i = 0; i < routines.Length; i++)
            {
                var part = partial.LongestRepeatFromStart().ToList();
                var max = part.Count;
                while (string.Join(',', part.ToStrings()).Length > maxLength)
                {
                    max--;
                    part = partial.LongestRepeatFromStart(max).ToList();
                }
                routines[i] = part;
                partial = partial.WithoutSequence(part).ToList();
            }
            if (partial.Count > 0) throw new Exception("Did not find path.");

            // Create input
            var pattern = moves.GetSequenceOrder(routines, "ABC");
            var c = Computer.From(InputLine);
            c[0] = 2;
            var data = new DataLink(c);
            data.InsertAscii(string.Join(',', pattern) + '\n');
            foreach (var routine in routines)
            {
                data.InsertAscii(string.Join(',', routine) + '\n');
            }
            data.InsertAscii("n\n");
            WriteLn(c.LastOutput());
        }
    }
}