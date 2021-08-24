using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day16 : Puzzle
    {
        public new string[][] Groups;
        public Dictionary<string, Interval[]> Rules = new();

        public Day16()
        {
            Groups = base.Groups.ToArray();
            ReadRules();
            Part = 2;
        }

        public void ReadRules()
        {
            var rules = Groups[0].Extract<(string, int, int, int, int)>(@"(.+): (\d+)-(\d+) or (\d+)-(\d+)");
            foreach (var (name, a, b, c, d) in rules)
            {
                Rules[name] = new[] {Interval.Range(a, b), Interval.Range(c, d)};
            }
        }

        public IEnumerable<int[]> AllTickets()
        {
            return Groups[2].Skip(1).Prepend(Groups[1][1]).Select(s => s.Csv().Ints().ToArray());
        }
        
        public override void PartOne()
        {
            var ranges = Rules.Values.Flatten().ToList();
            var count = AllTickets()
                .Flatten()
                .Where(i => !ranges.Any(range => range.ContainsInclusive(i)))
                .Sum();
            WriteLn(count);
        }

        public override void PartTwo()
        {
            var ranges = Rules.Values.Flatten().ToList();
            var tickets = AllTickets().Where(ticket => ticket.All(value => ranges.Any(range => range.ContainsInclusive(value)))).ToList();
            
            var possible = new OneToOne<int, string>();
            possible.AddKeys(Enumerable.Range(0, tickets[0].Length));
            possible.AddValues(Rules.Keys);
            
            possible.ReduceWithValid(i =>
            {
                var values = tickets.Select(ticket => ticket[i]).ToList();
                return Rules.WhereValue(posRanges => values.All(value => posRanges.Any(range => range.ContainsInclusive(value)))).Keys();
            });
            possible.ReduceToSingles();

            var myTicket = Groups[1][1].Csv().Ints().ToArray();
            var result = myTicket.Get(possible.Results.WhereValue(field => field.StartsWith("departure")).Keys()).LongProduct();
            WriteLn(result);
        }
    }
}