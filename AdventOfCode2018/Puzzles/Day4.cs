using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2018.Puzzles
{
    public class Day4 : Puzzle
    {
        public Day4()
        {
            Part = 2;
        }

        public IEnumerable<Entry> Entries()
        {
            return Input.Extract<Entry>(@"\[(?<Time>.+?)\] (?<Info>.+)");
        }

        public Dictionary<int, Line<int>> GetSchedule()
        {
            var sleep = new Dictionary<int, Line<int>>();
            var time = -1;
            Line<int> line = null;
            foreach (var update in Entries().OrderBy(entry => entry.Time))
            {
                if (update.Guard != -1)
                {
                    line = sleep.GetOrSetValue(update.Guard, () => new Line<int>());
                }
                else if (update.Sleep) time = update.Time.Minute;
                else line.Increment(time, update.Time.Minute - time);
            }
            return sleep;
        }

        public override void PartOne()
        {
            var schedule = GetSchedule();

            var (guard, minutes) = schedule.OrderByDescending(pair => pair.Value.Values.Sum()).First();
            var minute = minutes.OrderByDescending(pair => pair.Value).First().Key;
            WriteLn(guard * minute);
        }

        public override void PartTwo()
        {
            var schedule = GetSchedule();

            var (guard, minutes) = schedule.OrderByDescending(pair => pair.Value.Values.DefaultIfEmpty().Max()).First();
            var minute = minutes.OrderByDescending(pair => pair.Value).First().Key;
            WriteLn(guard * minute);
        }

        public class Entry
        {
            public DateTime Time { get; set; }
            public int Guard { get; set; } = -1;
            public bool Sleep { get; set; }

            public string Info
            {
                set
                {
                    if (value.Contains('#')) Guard = int.Parse(value.Split(' ')[1][1..]);
                    else Sleep = value == "falls asleep";
                }
            }
        }
    }
}