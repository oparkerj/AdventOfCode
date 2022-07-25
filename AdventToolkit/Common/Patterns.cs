using System.Text.RegularExpressions;

namespace AdventToolkit.Common;

public static class Patterns
{
    public static Regex Int2 => new(@"(?:\D+)?(\d+)\D+(\d+)(?:\D+)?");
    
    public static Regex Int3 => new(@"(?:\D+)?(\d+)\D+(\d+)\D+(\d+)(?:\D+)?");
}