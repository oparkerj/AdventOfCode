using System.Runtime.CompilerServices;
using AdventToolkit.New.Parsing.Core;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// Quick ways to invoke a parser.
/// </summary>
public static class ParseExtensions
{
    /// <summary>
    /// Parse a string using a segment parser.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TParse">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static T Parse<T, TParse>(this string s, SegmentParser<T, TParse> parser) => parser.Parse(s);
    
    /// <summary>
    /// Parse a string using a segment parser.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static T Parse<T>(this string s, SegmentParser<T, T> parser) => parser.Parse(s);

    /// <summary>
    /// Parse a string using a segment parser and a custom context.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TParse">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static T Parse<T, TParse>(this string s, IReadOnlyParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T, TParse> parser)
    {
        return parser.Parse(s);
    }
    
    /// <summary>
    /// Parse a string using a segment parser and a custom context.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static T Parse<T>(this string s, IReadOnlyParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T, T> parser)
    {
        return parser.Parse(s);
    }

    /// <summary>
    /// Parse a sequence of strings using a segment parser.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TParse">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T, TParse>(this IEnumerable<string> strings, SegmentParser<T, TParse> parser)
    {
        return parser.ParseMany(strings);
    }

    /// <summary>
    /// Parse a sequence of strings using a segment parser.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T>(this IEnumerable<string> strings, SegmentParser<T, T> parser)
    {
        return parser.ParseMany(strings);
    }

    /// <summary>
    /// Parse a sequence of strings using a segment parser and a custom context.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TParse">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T, TParse>(this IEnumerable<string> strings, IReadOnlyParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T, TParse> parser)
    {
        return parser.ParseMany(strings);
    }
    
    /// <summary>
    /// Parse a sequence of strings using a segment parser and a custom context.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T>(this IEnumerable<string> strings, IReadOnlyParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T, T> parser)
    {
        return parser.ParseMany(strings);
    }
}