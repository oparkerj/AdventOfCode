using System.Runtime.CompilerServices;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Core;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// Quick ways to invoke a parser.
/// </summary>
public static class ParseExtensions
{
    /// <summary>
    /// Parse a string without a format specifier.
    /// This effectively only performs the adapt step.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static T Parse<T>(this string s) => s.Parse<T>($"");

    /// <summary>
    /// Parse a string without a format specifier.
    /// This effectively only performs the adapt step.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TResolve">Ambiguous resolver.</typeparam>
    /// <returns></returns>
    public static T Parse<T, TResolve>(this string s) => s.Parse<T, TResolve>($"");

    /// <summary>
    /// Parse a string using a segment parser.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static T Parse<T>(this string s, SegmentParser<T> parser) => parser.Parse(s);

    /// <summary>
    /// Parse a string using a segment parser.
    /// </summary>
    /// <param name="s">Input string.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TResolve">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static T Parse<T, TResolve>(this string s, SegmentParser<T> parser)
    {
        parser.Context.SetupDisambiguation(typeof(TResolve));
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
    public static T Parse<T>(this string s, IParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T> parser)
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
    /// <typeparam name="TResolve">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static T Parse<T, TResolve>(this string s, IParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T> parser)
    {
        parser.Context.SetupDisambiguation(typeof(TResolve));
        return parser.Parse(s);
    }

    /// <summary>
    /// Parse a sequence of strings without a format specifier.
    /// This effectively only performs the adapt step.
    /// </summary>
    /// <param name="strings">Input string.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T>(this IEnumerable<string> strings) => strings.Parse<T>($"");

    /// <summary>
    /// Parse a sequence of strings without a format specifier.
    /// This effectively only performs the adapt step.
    /// </summary>
    /// <param name="strings">Input string.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TResolve">Ambiguous resolver.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T, TResolve>(this IEnumerable<string> strings) => strings.Parse<T, TResolve>($"");

    /// <summary>
    /// Parse a sequence of strings using a segment parser.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T>(this IEnumerable<string> strings, SegmentParser<T> parser)
    {
        return parser.ParseMany(strings);
    }

    /// <summary>
    /// Parse a sequence of strings using a segment parser.
    /// </summary>
    /// <param name="strings">Input strings.</param>
    /// <param name="parser">Segment parser.</param>
    /// <typeparam name="T">Parser output type.</typeparam>
    /// <typeparam name="TResolve">Parser adapt type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T, TResolve>(this IEnumerable<string> strings, SegmentParser<T> parser)
    {
        parser.Context.SetupDisambiguation(typeof(TResolve));
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
    public static IEnumerable<T> Parse<T>(this IEnumerable<string> strings, IParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T> parser)
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
    /// <typeparam name="TResolve">Parser output type.</typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Parse<T, TResolve>(this IEnumerable<string> strings, IParseContext context, [InterpolatedStringHandlerArgument("context")] SegmentParser<T> parser)
    {
        parser.Context.SetupDisambiguation(typeof(TResolve));
        return parser.ParseMany(strings);
    }

    /// <summary>
    /// Wrap a value as an instance of <see cref="AdaptValue{T}"/>.
    /// </summary>
    /// <param name="t">Value</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static AdaptValue<T> Adapt<T>(this T t) => new(t);
    
    /// <summary>
    /// Add a type lookup to the context.
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddLookup<T>(this IParseContext context)
        where T : ITypeLookup, new() =>
        context.AddType(new T());

    /// <summary>
    /// Add a type descriptor to the context.
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddType<T>(this IParseContext context)
        where T : ITypeDescriptor =>
        context.AddType(new TypeLookup<T>());
}