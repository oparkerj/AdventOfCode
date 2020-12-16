using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day16 : Puzzle
    {

        public string[][] Groups;
        public Dictionary<string, (int, int)[]> Rules = new();

        public Day16()
        {
            Groups = base.Groups.ToArray();
            ReadRules();
            Part = 2;
        }

        public void ReadRules()
        {
            var rules = Groups[0].FromFormat<(string, int, int, int, int)>(@"(.+): (\d+)-(\d+) or (\d+)-(\d+)");
            foreach (var (name, a, b, c, d) in rules)
            {
                Rules[name] = new[] {(a, b), (c, d)};
            }
        }

        public IEnumerable<int[]> AllTickets()
        {
            return Groups[2].Skip(1).Prepend(Groups[1][1]).Select(s => s.Csv().Ints().ToArray());
        }
        
        public override void PartOne()
        {
            var ranges = Rules.Values.SelectMany(tuples => tuples).ToList();
            var count = AllTickets()
                .SelectMany(ints => ints)
                .Where(i => !ranges.Any(range => range.Contains(i, true)))
                .Sum();
            WriteLn(count);
        }

        public override void PartTwo()
        {
            var possible = new Dictionary<int, string[]>();
            // Every range of values
            var ranges = Rules.Values.SelectMany(tuples => tuples).ToList();
            // Every valid ticket
            var tickets = AllTickets()
                .Where(ticket => ticket.All(i => ranges.Any(range => range.Contains(i, true))))
                .ToArray();
            var size = tickets[0].Length;
            for (var i = 0; i < size; i++)
            {
                // Any position could be any rule at the start
                possible[i] = Rules.Keys.ToArray();
            }
            // Filter out rules based on ranges
            foreach (var ticket in tickets)
            {
                for (var i = 0; i < size; i++)
                {
                    var v = ticket[i];
                    var updated = Rules.Where(pair => pair.Value.Any(range => range.Contains(v, true)))
                        .Select(pair => pair.Key)
                        .Intersect(possible[i])
                        .ToArray();
                    possible[i] = updated;
                }
            }
            // Filter remaining rules by looking at positions with only 1 possibility
            var done = new HashSet<int>();
            for (var i = 0; i < size - 1; i++)
            {
                // Get the position that has not been checked yet with only 1 possibility
                var (key, value) = possible.Single(pair => !done.Contains(pair.Key) && pair.Value.Length == 1);
                done.Add(key);
                var r = value[0];
                for (var j = 0; j < size; j++)
                {
                    if (j == key) continue;
                    possible[j] = possible[j].Where(s => s != r).ToArray();
                }
            }
            var myTicket = Groups[1][1].Csv().Ints().ToArray();
            var result = possible.Where(pair => pair.Value[0].StartsWith("departure"))
                .Select(pair => myTicket[pair.Key])
                .LongProduct();
            WriteLn(result);
        }
    }
}