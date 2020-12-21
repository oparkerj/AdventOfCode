using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day21 : Puzzle
    {
        public readonly List<(List<string> Ingredients, List<string> Allergens)> Foods = new();

        public Day21()
        {
            ReadInput();
            Part = 2;
        }

        public void ReadInput()
        {
            var foods = Input.Extract<(List<string>, List<string>)>("(?:(\\w+) )+\\(contains (?:(\\w+)\\W+)+");
            foreach (var food in foods)
            {
                Foods.Add(food);
            }
        }

        public IEnumerable<string> AllIngredients()
        {
            return Foods.Select(food => food.Ingredients).Flatten();
        }

        public IEnumerable<string> AllAllergens()
        {
            return Foods.Select(food => food.Allergens).Flatten();
        }

        public Dictionary<string, HashSet<string>> GetPossibleAllergens()
        {
            var possible = new Dictionary<string, HashSet<string>>();
            var all = AllIngredients().Distinct().ToArray();
            foreach (var ing in all)
            {
                possible[ing] = AllAllergens().ToHashSet();
            }
            foreach (var food in Foods)
            {
                foreach (var ing in all.Where(s => !food.Ingredients.Contains(s)))
                {
                    possible[ing].RemoveWhere(s => food.Allergens.Contains(s));
                }
            }
            return possible;
        }
        
        public override void PartOne()
        {
            var possible = GetPossibleAllergens();
            var result = AllIngredients().Count(s => possible[s].Count == 0);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var possible = GetPossibleAllergens();
            foreach (var ing in AllIngredients().Distinct().Where(s => possible[s].Count == 0))
            {
                possible.Remove(ing);
            }
            possible.MakeSingles();
            var ingredients = possible.OrderBy(pair => pair.Value.First()).Select(pair => pair.Key);
            WriteLn(string.Join(',', ingredients));
        }
    }
}