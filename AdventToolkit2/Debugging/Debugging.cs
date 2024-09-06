using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Debugging;

/// <summary>
/// Utilities for debugging.
/// </summary>
public static class Debugging
{
    private static bool _enableLogs;
    private static TraceListener? _logger;
    
    /// <summary>
    /// Enable printing of debug logs to the console.
    /// </summary>
    /// <param name="enable"></param>
    [Conditional("DEBUG")]
    public static void EnableLogs(bool enable = true)
    {
        if (_enableLogs == enable) return;
        
        _enableLogs = enable;
        if (_enableLogs)
        {
            Trace.Listeners.Add(_logger ??= new ConsoleTraceListener());
        }
        else
        {
            Trace.Listeners.Remove(_logger);
        }
    }

    /// <summary>
    /// Extension method to call the debugging ToString method.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string DebugString<T>(this T t) => ToString(t);

    /// <summary>
    /// Identity string function to shortcut the conversion.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ToString(string s) => s;

    /// <summary>
    /// String conversion function that produces more human-friendly results.
    /// For example, will perform the string conversion on each element of an enumerable.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static string ToString(object? o)
    {
        var builder = new StringBuilder();
        ToString(builder, o);
        return builder.ToString();
    }

    /// <summary>
    /// Add a human-friendly string to a builder.
    /// null values will result in the string "null"
    /// strings are added directly
    /// Tuples will perform string conversion on each element
    /// Types will display the simple type name.
    /// Dictionaries and enumerable types will convert each item to string.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="o"></param>
    public static void ToString(StringBuilder builder, object? o)
    {
        if (o is null)
        {
            builder.Append("null");
            return;
        }

        if (o is string s)
        {
            builder.Append(s);
            return;
        }
        
        if (o is ITuple tuple)
        {
            ToString(builder, tuple);
            return;
        }

        if (o is Type objType)
        {
            Types.AddSimpleName(objType, builder);
            return;
        }

        var type = o.GetType();
        
        if (type.TryGetTypeArguments(typeof(IDictionary<,>), out var dictionaryTypes))
        {
            var info = ((Action<StringBuilder, IDictionary<object?, object?>>) ToString).Method;
            info.WithGenericTypes(dictionaryTypes).Invoke(null, [builder, o]);
            return;
        }

        if (type.TryGetTypeArguments(typeof(IEnumerable<>), out var enumerableTypes))
        {
            var info = ((Action<StringBuilder, IEnumerable<object?>>) ToString).Method;
            info.WithGenericTypes(enumerableTypes).Invoke(null, [builder, o]);
            return;
        }

        builder.Append(o);
    }

    /// <summary>
    /// Special string conversion for tuples.
    /// To assist with <see cref="ToString(object?)"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="tuple"></param>
    public static void ToString(StringBuilder builder, ITuple tuple)
    {
        builder.Append('(');

        if (tuple.Length > 0)
        {
            ToString(builder, tuple[0]);
        }
        for (var i = 1; i < tuple.Length; i++)
        {
            builder.Append(", ");
            ToString(builder, tuple[i]);
        }
        
        builder.Append(')');
    }

    /// <summary>
    /// Special string conversion for enumerable types.
    /// To assist with <see cref="ToString(object?)"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    public static void ToString<T>(StringBuilder builder, IEnumerable<T> items)
    {
        builder.Append('[');
        
        using var e = items.GetEnumerator();
        if (e.MoveNext())
        {
            ToString(builder, e.Current);
        }
        while (e.MoveNext())
        {
            builder.Append(", ");
            ToString(builder, e.Current);
        }
        
        builder.Append(']');
    }

    /// <summary>
    /// Special string conversion for dictionaries.
    /// To assist with <see cref="ToString(object?)"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="dictionary"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static void ToString<TKey, TValue>(StringBuilder builder, IDictionary<TKey, TValue> dictionary)
    {
        builder.Append('{');

        using var e = dictionary.GetEnumerator();
        if (e.MoveNext())
        {
            ToString(builder, e.Current.Key);
            builder.Append(" = ");
            ToString(builder, e.Current.Value);
        }
        while (e.MoveNext())
        {
            builder.Append(", ");
            ToString(builder, e.Current.Key);
            builder.Append(" = ");
            ToString(builder, e.Current.Value);
        }
        
        builder.Append('}');
    }

    /// <summary>
    /// Test if a number can be converted to another number type via truncation.
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public static bool CanTruncate<TIn, TOut>(this TIn input)
        where TIn : INumber<TIn>
        where TOut : INumber<TOut>
    {
        return TOut.CreateTruncating(input) == TOut.CreateSaturating(input);
    }
}