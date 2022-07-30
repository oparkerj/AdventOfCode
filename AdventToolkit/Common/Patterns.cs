using System.Linq;
using System.Text.RegularExpressions;

namespace AdventToolkit.Common;

public static class Patterns
{
    public static Regex NonDigit => new(@"[^0-9\-]");

    public static Regex Int => new(@"(-?\d+)");

    public static Regex IntPattern(int amount) => new(string.Join($"{NonDigit}+", Enumerable.Repeat(Int.ToString(), amount)));
    
    public static Regex Int2 => IntPattern(2);
    
    public static Regex Int3 => IntPattern(3);
    
    public static Regex Int4 => IntPattern(4);
    
    public static Regex Int5 => IntPattern(5);
}