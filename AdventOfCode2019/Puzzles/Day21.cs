using AdventOfCode2019.IntCode;
using AdventToolkit;

namespace AdventOfCode2019.Puzzles
{
    public class Day21 : Puzzle
    {
        public Day21()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var c = Computer.From(InputLine);
            var data = new DataLink(c);
            data.InsertAscii(
@"NOT C T
OR D J
AND T J
NOT A T
OR T J
WALK
");
            WriteLn(c.LastOutput());
        }

        public override void PartTwo()
        {
            var c = Computer.From(InputLine);
            var data = new DataLink(c);
            data.InsertAscii(
@"NOT C T
NOT B J
OR T J
AND D J
AND H J
NOT A T
OR T J
RUN
");
            WriteLn(c.LastOutput());
        }
    }
}