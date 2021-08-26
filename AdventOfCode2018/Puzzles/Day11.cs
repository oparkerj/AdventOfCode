using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2018.Puzzles
{
    public class Day11 : Puzzle
    {
        public readonly int Serial;

        public Day11()
        {
            Serial = InputLine.AsInt();
            Part = 2;
        }

        public int PowerLevel(Pos pos)
        {
            var rack = pos.X + 10;
            var power = (rack * pos.Y + Serial) * rack;
            return power.Digits().ElementAtOrDefault(2) - 5;
        }
        
        public override void PartOne()
        {
            var result = Algorithms.Sequences(2, 300 - 2)
                .ToPositions()
                .SelectMaxBy(pos => new Rect(pos, 3, 3).Positions().Select(PowerLevel).Sum());
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var result = Enumerable.Range(1, 300).SelectMany(i => Algorithms.Sequences(2, 300 - i + 1).ToPositions().Select(pos => pos.To3D(i)))
                .SelectMaxBy(square => new Rect(square.To2D, square.Z, square.Z).Positions().Select(PowerLevel).Sum());
            WriteLn(result);
        }
    }
}