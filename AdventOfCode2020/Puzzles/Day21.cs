using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day21 : Puzzle
    {
        public List<Food> Foods = new();

        public Day21()
        {
            ReadInput();
            Part = 2;
        }

        public void ReadInput()
        {
            var foods = Input.Extract<(List<string>, List<string>)>("(?:(\\w+) )+\\(contains (?:(\\w+)\\W+)+");
            foreach (var (i, a) in foods)
            {
                Foods.Add(new Food(i, a));
            }
        }

        public class Food
        {
            public readonly List<string> Ingredients;
            public readonly List<string> Allergens;

            public Food(List<string> ingredients, List<string> allergens)
            {
                Ingredients = ingredients;
                Allergens = allergens;
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
        
        public override void PartOne()
        {
            var possible = new Dictionary<string, List<string>>();
            var all = AllIngredients().ToArray();
            foreach (var ing in all)
            {
                possible[ing] = AllAllergens().ToList();
            }
            foreach (var food in Foods)
            {
                foreach (var ing in all.Where(s => !food.Ingredients.Contains(s)))
                {
                    possible[ing].RemoveAll(s => food.Allergens.Contains(s));
                }
            }

            var result = AllIngredients().Count(s => possible[s].Count == 0);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var possible = new Dictionary<string, HashSet<string>>();
            var all = AllIngredients().ToArray();
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
            foreach (var ing in AllIngredients().Distinct().Where(s => possible[s].Count == 0))
            {
                possible.Remove(ing);
            }

            var remain = possible.Keys.ToArray();
            var done = new HashSet<string>();
            for (var i = 0; i < possible.Count - 1; i++)
            {
                var (key, value) = possible.Single(pair => !done.Contains(pair.Key) && pair.Value.Count == 1);
                done.Add(key);
                var r = value.First();
                foreach (var ing in remain)
                {
                    if (ing == key) continue;
                    possible[ing].Remove(r);
                }
            }

            var ingredients = possible.OrderBy(pair => pair.Value.First()).Select(pair => pair.Key);
            WriteLn(string.Join(',', ingredients));
        }
    }
}