using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2021.Puzzles
{
    public class Day6 : Puzzle
    {
        public Day6()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var timers = InputLine.Csv().Ints().ToList();

            void Step()
            {
                var len = timers.Count;
                for (var i = 0; i < len; i++)
                {
                    if (timers[i]-- == 0)
                    {
                        timers[i] = 6;
                        timers.Add(8);
                    }
                }
            }

            for (var i = 0; i < 80; i++)
            {
                Step();
            }
            
            Clip(timers.Count);
        }

        public override void PartTwo()
        {
            var counts = InputLine.Csv().Ints().Frequencies().ToArrayByIndex(9, pair => pair.Key, pair => (long) pair.Value);
            var temp = new long[counts.Length];

            void Step()
            {
                Array.Copy(counts, 1, temp, 0, counts.Length - 1);
                temp[^1] = counts[0];
                temp[6] += counts[0];
                (counts, temp) = (temp, counts);
            }

            for (var i = 0; i < 256; i++)
            {
                Step();
            }
            
            Clip(counts.Sum());
        }
    }
}