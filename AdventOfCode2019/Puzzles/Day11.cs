using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019.Puzzles
{
    public class Day11 : Puzzle
    {
        public Grid<int> Map = new();
        public Pos Bot = Pos.Origin;
        public Pos Dir = Pos.Up;

        public Day11()
        {
            Part = 2;
        }

        public void RunBot()
        {
            var c = Computer.From(InputLine);
            c.LineIn = () => Map[Bot];
            c.LineOut = new OutputSequence()
                .ThenInt(data => Map[Bot] = data)
                .ThenBool(dir => Dir = dir ? Dir.Clockwise() : Dir.CounterClockwise())
                .And(() => Bot += Dir)
                .Line;
            c.Execute();
        }
        
        public override void PartOne()
        {
            RunBot();
            WriteLn(Map.Count);
        }

        public override void PartTwo()
        {
            Map[Bot] = 1;
            RunBot();
            WriteLn(Map.ToArray(false).Stringify(i => i == 1 ? '#' : ' '));
        }
    }
}