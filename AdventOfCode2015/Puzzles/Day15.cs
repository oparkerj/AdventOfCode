using AdventToolkit;
using AdventToolkit.Common;
using RegExtract;
using Z3Helper;

namespace AdventOfCode2015.Puzzles;

public class Day15 : Puzzle
{
    public record Ingredient(string Name, int Capacity, int Durability, int Flavor, int Texture, int Calories);

    public ZExpr ComponentValue(IList<Ingredient> ingredients, IList<ZExpr> exprs, Func<Ingredient, int> component)
    {
        var sum = exprs.Select((expr, i) => (expr, amount: component(ingredients[i])))
            .Where(tuple => tuple.amount != 0)
            .Select(tuple => tuple.expr * tuple.amount)
            .Sum();
        return sum.Min(0);
    }
    
    public override void PartOne()
    {
        var ingredients = Input.Extract<Ingredient>(@$"(\w+){Patterns.NonInt}+{Patterns.Int5}").ToList();
        var vars = ingredients.Select(i => i.Name.IntConst()).ToArray();

        var capacity = ComponentValue(ingredients, vars, i => i.Capacity);
        var durability = ComponentValue(ingredients, vars, i => i.Durability);
        var flavor = ComponentValue(ingredients, vars, i => i.Flavor);
        var texture = ComponentValue(ingredients, vars, i => i.Texture);

        var total = capacity * durability * flavor * texture;

        var o = Zzz.Optimize();
        // Set constraints
        o.AddAll(vars, expr => expr >= 0);
        o.Add(vars.Sum() == 100);
        if (Part == 2)
        {
            o.Add(ComponentValue(ingredients, vars, i => i.Calories) == 500);
        }
        
        o.LogResult(o.Maximize(total));
    }
}