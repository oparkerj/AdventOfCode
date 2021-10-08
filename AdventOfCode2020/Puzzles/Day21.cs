using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day21 : Puzzle
    {
        public List<(List<string> Ingredients, List<string> Allergens)> Foods;

        public Day21()
        {
            ReadInput();
            // Part = 2;
        }

        public void ReadInput()
        {
            Foods = Input.Extract<(List<string>, List<string>)>(@"(?:(\w+) )+\(contains (?:(\w+)\W+)+").ToList();
        }

        public OneToOne<string, string> GetFoods()
        {
            var possible = new OneToOne<string, string>();
            foreach (var (ingredients, allergens) in Foods)
            {
                possible.AddKeys(ingredients);
                possible.AddValues(allergens);
            }
            foreach (var (ingredients, allergens) in Foods)
            {
                possible.ValuesPresentInKeys(ingredients, allergens);
            }
            return possible;
        }

        public override void PartOne()
        {
            var possible = GetFoods();
            var none = possible.Options.WhereValue(options => options.Count == 0).Keys().ToList();
            WriteLn(Foods.Select(tuple => tuple.Ingredients).Select(list => list.Count(none.Contains)).Sum());
        }

        public override void PartTwo()
        {
            var possible = GetFoods();
            possible.RemoveExtra();
            possible.ReduceToSingles();
            WriteLn(possible.Results.OrderBy(pair => pair.Value).Keys().ToCsv());
        }
    }
}