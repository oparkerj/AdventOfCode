using System.Numerics;
using AdventToolkit2.Parsing.Builtin;
using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Parse support for strings.
/// </summary>
public class StringParse : ITypeLookup, IParserLookupByInput<string>, IAdapterLookup<string>
{
    public bool Match(Type type) => type == typeof(string);

    public bool PassiveSelect => true;

    public bool TryCollect(Type type, Type inner, IParseContext context, out IParser collector)
    {
        if (inner == typeof(char))
        {
            collector = new CharsToString();
            return true;
        }

        collector = default!;
        return false;
    }

    public bool TryGetCollectType(Type type, IParseContext context, out Type inner)
    {
        inner = typeof(char);
        return true;
    }
    
    // Builder values
    public bool TryLookup<T>(T value, string extra, IParseContext context, out IParser parser)
    {
        // char value will split a string
        if (value is char ch)
        {
            // Special case for newlines
            if (ch == '\n')
            {
                parser = new LineSplit();
                return true;
            }
            
            parser = new CharSplit(ch)
            {
                Trim = extra.Contains('t'),
                RemoveEmpty = extra.Contains('r')
            };
            return true;
        }

        parser = default!;
        return false;
    }

    // Adapter lookup
    public bool TryLookup(Type to, IParseContext context, out IParser parser)
    {
        if (to == typeof(string))
        {
            parser = IdentityAdapter<string>.Instance;
            return true;
        }
        
        // Use custom implementation for char instead of IParsable.
        if (to == typeof(char))
        {
            parser = new ToChar();
            return true;
        }
        
        // string can be converted to any type implementing IParsable<>
        if (to.TryGetTypeArguments(typeof(IParsable<>), out var parsableTypes))
        {
            parser = typeof(ToParsable<>).NewParserGeneric([parsableTypes[0]]);
            return true;
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Convert sequence of chars to string.
    /// </summary>
    public class CharsToString : IParser<IEnumerable<char>, string>
    {
        public string Parse(IEnumerable<char> input) => string.Concat(input);
    }

    /// <summary>
    /// Split a string by a character
    /// </summary>
    public class CharSplit(char split) : IParser<string, string[]>
    {
        /// <summary>
        /// Split character.
        /// </summary>
        public readonly char Split = split;

        private readonly StringSplitOptions _options;

        /// <summary>
        /// Whether to trim the split sections
        /// </summary>
        public bool Trim
        {
            get => _options.HasFlag(StringSplitOptions.TrimEntries);
            init
            {
                if (value)
                {
                    _options |= StringSplitOptions.TrimEntries;
                }
            }
        }

        /// <summary>
        /// Whether empty sections are removed.
        /// </summary>
        public bool RemoveEmpty
        {
            get => _options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
            init
            {
                if (value)
                {
                    _options |= StringSplitOptions.RemoveEmptyEntries;
                }
            }
        }

        public string[] Parse(string input) => input.Split(Split, _options);
    }

    /// <summary>
    /// Split a string by lines.
    /// </summary>
    public class LineSplit : IParser<string, List<string>>
    {
        public List<string> Parse(string input)
        {
            var result = new List<string>();
            foreach (var line in input.AsSpan().EnumerateLines())
            {
                result.Add(line.ToString());
            }
            return result;
        }
    }

    /// <summary>
    /// Convert a string to a parsable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToParsable<T> : IParser<string, T>
        where T : IParsable<T>
    {
        public T Parse(string input) => T.Parse(input, null);
    }

    /// <summary>
    /// Special case converting string to char.
    /// This will just take the first character, whereas the built-in implementation
    /// will throw if the string is not exactly one character long.
    /// </summary>
    public class ToChar : IParser<string, char>
    {
        public char Parse(string input) => input[0];
    }
}