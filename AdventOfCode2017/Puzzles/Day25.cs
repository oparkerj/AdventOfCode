using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Automata;
using RegExtract;

namespace AdventOfCode2017.Puzzles
{
    public class Day25 : Puzzle
    {
        public TuringMachine<bool> ReadTm()
        {
            var tm = new TuringMachine<bool>();
            var states = new Dictionary<char, int>();
            states[AllGroups[0][0][^2]] = tm.NewState();
            foreach (var c in AllGroups.Skip(1).Select(s => s[0][^2]))
            {
                if (!states.ContainsKey(c)) states[c] = tm.NewState();
            }
            foreach (var instruction in AllGroups.Skip(1))
            {
                var state = states[instruction[0][^2]];
                for (var c = 0; c < 2; c++)
                {
                    var @case = new Span<string>(instruction, c * 4 + 1, 4);
                    var current = @case[0][^2] == '1';
                    var write = @case[1][^2] == '1';
                    var dir = @case[2][^3] == 'h';
                    var next = states[@case[3][^2]];
                    tm[state].Add(new Single<bool>(current, next));
                    tm.Update[(state, current)] = (write, dir);
                }
            }
            return tm;
        }

        public override void PartOne()
        {
            var count = Input[1].Extract<int>(@"\D+(\d+).+");
            var tm = ReadTm();
            tm.RunSteps(count);
            var result = tm.Tape.CountValues(true);
            WriteLn(result);
        }
    }
}