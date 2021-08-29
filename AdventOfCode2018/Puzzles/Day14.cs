using System;
using System.Text;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles
{
    public class Day14 : Puzzle
    {
        public readonly StringBuilder Scores = new("37");
        public int A = 0;
        public int B = 1;
        
        public Day14()
        {
            Part = 2;
        }

        public void Step()
        {
            var a = Scores[A].AsInt();
            var b = Scores[B].AsInt();
            Scores.Append(a + b);
            A = (A + a + 1) % Scores.Length;
            B = (B + b + 1) % Scores.Length;
        }

        public override void PartOne()
        {
            var input = InputLine.AsInt();
            var target = input + 10;
            
            while (Scores.Length < target)
            {
                Step();
            }

            WriteLn(Scores.ToString(input, InputLine.Length));
        }

        public override void PartTwo()
        {
            var input = InputLine;
            var check = input.Length + 1;

            while (Scores.Length < check || !Scores.ToString(Scores.Length - check, check).Contains(input))
            {
                Step();
            }
            
            WriteLn(Scores.ToString().IndexOf(input, StringComparison.Ordinal));
        }
    }
}