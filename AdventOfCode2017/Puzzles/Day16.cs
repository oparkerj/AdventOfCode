using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles
{
    public class Day16 : Puzzle
    {
        public Day16()
        {
            Part = 2;
        }

        public void Dance(CircularBuffer<int> buf)
        {
            var offset = 0;
            foreach (var cmd in InputLine.Csv())
            {
                if (cmd[0] == 's')
                {
                    offset -= int.Parse(cmd[1..]);
                }
                else if (cmd[0] == 'x')
                {
                    var i = cmd.IndexOf('/');
                    var (a, b) = (int.Parse(cmd[1..i]), int.Parse(cmd[(i + 1)..]));
                    (buf[a + offset], buf[b + offset]) = (buf[b + offset], buf[a + offset]);
                }
                else if (cmd[0] == 'p')
                {
                    var (a, b) = (buf.IndexOf(cmd[1] - 'a'), buf.IndexOf(cmd[3] - 'a'));
                    (buf[a], buf[b]) = (buf[b], buf[a]);
                }
            }
            buf.RotateTo(offset);
        }

        public string GetOrder(CircularBuffer<int> buf)
        {
            return buf.Select(i => (char) (i + 'a')).Str();
        }

        public override void PartOne()
        {
            var buf = new CircularBuffer<int>(Enumerable.Range(0, 16).ToArray());
            Dance(buf);
            var result = GetOrder(buf);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var buf = new CircularBuffer<int>(Enumerable.Range(0, 16).ToArray());
            var (offset, cycle) = Algorithms.FindCyclePeriod(buf, GetOrder, Dance);
            var realOffset = 1_000_000_000.CycleOffset((int) offset, (int) cycle);
            buf = new CircularBuffer<int>(Enumerable.Range(0, 16).ToArray());
            buf.RepeatAction(Dance, realOffset);
            var result = GetOrder(buf);
            WriteLn(result);
        }
    }
}