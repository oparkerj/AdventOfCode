using System.Diagnostics;
using AdventToolkit.New.Debugging;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// General parse library functions.
/// </summary>
public static class Parse
{
    /// <summary>
    /// Category used when the parse library prints debug messages.
    /// </summary>
    public const string Category = "Parse";

    /// <summary>
    /// Output debug information from within the parser.
    /// </summary>
    /// <param name="str"></param>
    [Conditional("DEBUG")]
    internal static void Verbose(SimpleTypeString str)
    {
        Debug.WriteLine(str.ToStringAndDispose(), Category);
    }
    
    /// <summary>
    /// Output debug information if a condition is true.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="str"></param>
    [Conditional("DEBUG")]
    internal static void VerboseIf(bool condition, SimpleTypeString str)
    {
        Debug.WriteLineIf(condition, str.ToStringAndDispose(), Category);
    }
    
    /// <summary>
    /// If a condition is true, output a debug message, otherwise output a different debug message.
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="strTrue"></param>
    /// <param name="strFalse"></param>
    [Conditional("DEBUG")]
    internal static void VerboseIf(bool condition, SimpleTypeString strTrue, SimpleTypeString strFalse)
    {
        Verbose(condition ? strTrue : strFalse);
    }
}