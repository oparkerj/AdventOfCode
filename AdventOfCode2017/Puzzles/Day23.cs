using AdventToolkit;

namespace AdventOfCode2017.Puzzles
{
    public class Day23 : Puzzle
    {
        public Day23()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var program = new Day18.Program(Input);
            var result = 0;
            program.Debug = s => result += s == "mul" ? 1 : 0;
            program.Run();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var count = 0;
            for (var i = 108400; i <= 125400; i += 17)
            {
                for (var j = 2; j < i; j++)
                {
                    if (i % j == 0)
                    {
                        count++;
                        break;
                    }
                }
            }
            WriteLn(count);
        }
    }
}