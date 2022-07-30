using System.Text.RegularExpressions;

namespace AdventToolkit.Common;

public static class Patterns
{
    public static Regex Nd => new(@"[^0-9\-]");
    
    public static Regex Int2 => new(@$"(-?\d+){Nd}+(-?\d+)");
    
    public static Regex Int3 => new(@$"(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+)");
    
    public static Regex Int4 => new(@$"(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+)");
    
    public static Regex Int5 => new(@$"(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+){Nd}+(-?\d+)");
}