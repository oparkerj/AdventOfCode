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
        var sum = exprs.Select((expr, i) => (expr, component(ingredients[i])))
            .Where(tuple => tuple.Item2 != 0)
            .Select(tuple => tuple.expr * tuple.Item2)
            .Sum();
        return (sum > 0).Condition(sum, 0);
    }
    
    public override void PartOne()
    {
        var ingredients = Input.Extract<Ingredient>(@"(\w+)\D+" + Patterns.Int5).ToList();
        var vars = ingredients.Select(i => i.Name.IntConst()).ToArray();

        var capacity = ComponentValue(ingredients, vars, i => i.Capacity);
        var durability = ComponentValue(ingredients, vars, i => i.Durability);
        var flavor = ComponentValue(ingredients, vars, i => i.Flavor);
        var texture = ComponentValue(ingredients, vars, i => i.Texture);

        var total = capacity * durability * flavor * texture;

        var o = Zzz.Optimize();
        o.AddAll(vars, expr => expr >= 0);
        o.Add(vars.Sum() == 100);
        if (Part == 2)
        {
            o.Add(ComponentValue(ingredients, vars, i => i.Calories) == 500);
        }

        var result = o.Maximize(total);
        WriteLn(o.Check());
        WriteLn(result.Value);
    }
}