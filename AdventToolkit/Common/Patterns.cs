using System.Linq;
using System.Text.RegularExpressions;

namespace AdventToolkit.Common;

public static partial class Patterns
{
    public const string WordStr = @"(\w+)";

    [GeneratedRegex(WordStr)]
    public static partial Regex GetWord();
    
    public static Regex Word => GetWord();

    public const string NonIntStr = "[^-+0-9]";

    [GeneratedRegex(NonIntStr)]
    public static partial Regex GetNonInt();

    public static Regex NonInt => GetNonInt();

    public const string IntStr = @"((?:-|\+)?\d+)";

    [GeneratedRegex(IntStr)]
    public static partial Regex GetInt();

    public static Regex Int => GetInt();

    public const string UIntStr = @"(\d+)";

    [GeneratedRegex(UIntStr)]
    public static partial Regex GetUInt();
    
    public static Regex UInt => GetUInt();

    public const string IntListStr = @$"(?:{NonIntStr}*{IntStr}{NonIntStr}*)+";

    [GeneratedRegex(IntListStr)]
    public static partial Regex GetIntList();

    public static Regex IntList => GetIntList();

    public static Regex IntPattern(int amount) => new(string.Join($"{NonInt}+", Enumerable.Repeat(Int.ToString(), amount)));
    
    public static Regex UIntPattern(int amount) => new(string.Join(@"\D+", Enumerable.Repeat(UInt.ToString(), amount)));

    private const string Sep = $@".{NonIntStr}*";

    private const string USep = @"\D+";

    [GeneratedRegex($"{IntStr}{Sep}{IntStr}")]
    public static partial Regex GetInt2();
    
    public static Regex Int2 => GetInt2();

    [GeneratedRegex($"{UIntStr}{USep}{UIntStr}")]
    public static partial Regex GetUInt2();
    
    public static Regex UInt2 => GetUInt2();
    
    [GeneratedRegex($"{IntStr}{Sep}{IntStr}{Sep}{IntStr}")]
    public static partial Regex GetInt3();

    public static Regex Int3 => GetInt3();
    
    [GeneratedRegex($"{UIntStr}{USep}{UIntStr}{USep}{UIntStr}")]
    public static partial Regex GetUInt3();
    
    public static Regex UInt3 => GetUInt3();
    
    [GeneratedRegex($"{IntStr}{Sep}{IntStr}{Sep}{IntStr}{Sep}{IntStr}")]
    public static partial Regex GetInt4();
    
    public static Regex Int4 => GetInt4();
    
    [GeneratedRegex($"{UIntStr}{USep}{UIntStr}{USep}{UIntStr}{USep}{UIntStr}")]
    public static partial Regex GetUInt4();
    
    public static Regex UInt4 => GetUInt4();
    
    [GeneratedRegex($"{IntStr}{Sep}{IntStr}{Sep}{IntStr}{Sep}{IntStr}{Sep}{IntStr}")]
    public static partial Regex GetInt5();

    public static Regex Int5 => GetInt5();
    
    [GeneratedRegex($"{UIntStr}{USep}{UIntStr}{USep}{UIntStr}{USep}{UIntStr}{USep}{UIntStr}")]
    public static partial Regex GetUInt5();
    
    public static Regex UInt5 => GetUInt5();
}