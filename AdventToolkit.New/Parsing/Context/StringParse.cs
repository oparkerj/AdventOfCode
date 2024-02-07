using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

public class StringParse : ITypeDescriptor, IParserLookupByInput<string>, IAdapterLookup<string>
{
    public bool Match(Type type) => type == typeof(string);

    public bool PassiveSelect => true;

    public bool TryCollect(Type type, Type inner, IReadOnlyParseContext context, out IParser constructor)
    {
        if (inner == typeof(char))
        {
            constructor = new CharsToString();
            return true;
        }

        constructor = default!;
        return false;
    }

    public bool TryGetCollectType(Type type, IReadOnlyParseContext context, out Type inner)
    {
        inner = typeof(char);
        return true;
    }
    
    // Builder values
    public bool TryLookup<T>(T value, string extra, IReadOnlyParseContext context, out IParser parser)
    {
        // char value will split a string
        if (value is char ch)
        {
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
    public bool TryLookup(Type to, IReadOnlyParseContext context, out IParser parser)
    {
        // string can be converted to any type implementing IParsable<>
        if (to.TryGetTypeArguments(typeof(IParsable<>), out var parsableTypes))
        {
            parser = typeof(ToParsable<>).NewParserGeneric([parsableTypes[0]]);
            return true;
        }

        parser = default!;
        return false;
    }

    public class CharsToString : IParser<IEnumerable<char>, string>
    {
        public string Parse(IEnumerable<char> input) => string.Concat(input);
    }

    public class CharSplit : IParser<string, string[]>
    {
        public readonly char Split;

        private readonly StringSplitOptions _options;

        public CharSplit(char split) => Split = split;

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

    public class ToParsable<T> : IParser<string, T>
        where T : IParsable<T>
    {
        public T Parse(string input) => T.Parse(input, null);
    }
}