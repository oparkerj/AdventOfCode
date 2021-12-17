using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions;

public static class Conversions
{
    public static int AsInt(this string s) => int.Parse(s);
        
    public static int AsInt(this char c) => c - '0';

    public static int AsInt(this bool b) => b ? 1 : 0;
        
    public static bool AsBool(this int i) => i != 0;
        
    public static bool AsBool(this long i) => i != 0;

    public static IEnumerable<int> AsInts(this IEnumerable<bool> bools) => bools.Select(AsInt);

    public static IEnumerable<bool> AsBools(this IEnumerable<int> ints) => ints.Select(AsBool);
}