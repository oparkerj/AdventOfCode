using System.Linq;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019.Puzzles
{
    public class Day19 : Puzzle
    {
        public Grid<bool> Area = new();
        
        public Day19()
        {
            Part = 2;
        }

        public bool Pulls(Pos pos)
        {
            if (Area.Has(pos)) return Area[pos];
            var c = Computer.From(InputLine);
            using var input = new ComputerInput(pos.X, pos.Y);
            c.LineIn = input.Line;
            return Area[pos] = c.NextBool();
        }

        public override void PartOne()
        {
            const int Size = 50;
            var count = Rect.Size(Size, Size).Positions().Count(Pulls);
            WriteLn(count);
        }

        public override void PartTwo()
        {
            Run(PartOne);
            const int Size = 100;
            const int Target = (Size - 1) * 2;
            var right = Area.WhereValue(true).Keys().OrderByDescending(pos => pos.MDist(Pos.Origin)).First();
            var up = right;
            while (right.MDist(up) < Target || !(Pulls(right) && Pulls(up)))
            {
                right += Pulls(right) ? Pos.Right : Pos.Up;
                up += Pulls(up) ? Pos.Up : Pos.Right;
            }
            var min = new Rect(right, up).Min;
            WriteLn(min.X * 10000 + min.Y);
        }
    }
}