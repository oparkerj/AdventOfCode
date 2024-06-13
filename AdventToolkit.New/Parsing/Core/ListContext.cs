using AdventToolkit.New.Debugging;
using AdventToolkit.New.Parsing.Disambiguation;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// Context which stores everything in a list. Matches are done by returning
/// the first item from the list that matches.
/// </summary>
public class ListContext : IParseContext
{
    private readonly List<ITypeLookup> _types = [];
    private readonly List<IParserLookup> _parserLookups = [];
    private readonly List<IModifier> _modifiers = [];
    private readonly List<IAdapterLookup> _adapterLookups = [];

    private Stack<DisambiguationSection>? _disambiguation;
    private bool _disambiguationComplete;

    public IEnumerable<ITypeLookup> Types => _types;
    public IEnumerable<IParserLookup> ParserLookups => _parserLookups;
    public IEnumerable<IModifier> Modifiers => _modifiers;
    public IEnumerable<IAdapterLookup> AdapterLookups => _adapterLookups;

    /// <summary>
    /// Stores the types and index of the current disambiguation state.
    /// This is effectively storing where we are in the current tuple.
    /// </summary>
    /// <param name="Parts">Current types.</param>
    /// <param name="Index">Current index.</param>
    private readonly record struct DisambiguationSection(Type[] Parts, int Index = 0)
    {
        public ref Type Current => ref Parts[Index];
    }

    /// <summary>
    /// Enter into any tuples to find the real current type.
    /// Initialize the completion state.
    /// </summary>
    private void EnterSection()
    {
        // Are there any disambiguations?
        if (!(_disambiguation?.Count > 0)) return;

        // Nest into tuples
        while (_disambiguation.Peek() is {Current: var current} && current.IsTupleType())
        {
            _disambiguation.Push(new DisambiguationSection(current.GetGenericArguments()));
        }
        
        ApplyAuto(ref _disambiguation.Peek().Current);
        _disambiguationComplete = _disambiguation.Peek().Current == typeof(Null);
    }

    /// <summary>
    /// Apply the current type until it is no longer an <see cref="IAutoDisambiguation"/>.
    /// </summary>
    /// <param name="type"></param>
    private void ApplyAuto(ref Type type)
    {
        while (type.IsAssignableTo(typeof(IAutoDisambiguation)))
        {
            type = IDisambiguation.ApplyTo(type) ?? typeof(Null);
        }
    }

    public void SetupDisambiguation(Type type)
    {
        if (_disambiguation is null)
        {
            _disambiguation = new Stack<DisambiguationSection>();
        }
        else
        {
            _disambiguation.Clear();
        }

        var parts = type.IsTupleType() ? type.GetGenericArguments() : [type];
        _disambiguation.Push(new DisambiguationSection(parts));
        EnterSection();
    }

    public bool ApplyDisambiguation(Type? type)
    {
        Parse.Verbose($"Attempting to apply disambiguation {type}");
        
        // Skip if no state or all sections used
        if (_disambiguation?.Count is null or 0) return type is null;

        var current = _disambiguation.Peek();

        // Null type will advance to the next section
        if (type is null)
        {
            if (!_disambiguationComplete)
            {
                Err.InvalidOperation("Disambiguation was not fully applied when advancing state.");
            }

            // Find the next type
            while (true)
            {
                _disambiguation.Pop();
                if (current.Index + 1 >= current.Parts.Length)
                {
                    // If the index reaches the end, keep going until we find a section to advance
                    if (!_disambiguation.TryPeek(out current)) break;
                }
                else
                {
                    _disambiguation.Push(current with {Index = current.Index + 1});
                    break;
                }
            }

            // This will push new sections if the next type is a tuple
            EnterSection();
            return true;
        }

        // Otherwise, if the type matches, update the current state
        if (!IDisambiguation.Matches(current.Current, type)) return false;
        
        current.Current = IDisambiguation.ApplyTo(current.Current) ?? typeof(Null);
        ApplyAuto(ref current.Current);
        _disambiguationComplete = current.Current == typeof(Null);
        return true;
    }

    public virtual bool TryLookupType(Type type, out ITypeLookup descriptor)
    {
        foreach (var typeDescriptor in _types)
        {
            if (!typeDescriptor.Match(type)) continue;
            descriptor = typeDescriptor;
            return true;
        }
        descriptor = default!;
        return false;
    }

    public virtual bool TryLookupParser<T>(Type inputType, T value, string extra, out IParser parser)
    {
        foreach (var parserLookup in _parserLookups)
        {
            if (parserLookup.TryLookup(inputType, value, extra, this, out parser)) return true;
        }
        parser = default!;
        return false;
    }

    public virtual bool TryLookupAdapter(Type from, Type to, out IParser parser)
    {
        foreach (var adapterLookup in _adapterLookups)
        {
            if (adapterLookup.TryLookup(from, to, this, out parser)) return true;
        }
        parser = default!;
        return false;
    }

    public virtual bool TryApplyModifier<TPipeline, T>(TPipeline pipeline, Type inputType, T value, string extra)
    {
        foreach (var modifier in _modifiers)
        {
            if (modifier.TryApply(pipeline, inputType, value, extra, this)) return true;
        }
        return false;
    }

    public virtual bool TryCollect(Type container, Type inner, out IParser constructor)
    {
        foreach (var typeDescriptor in _types)
        {
            if (typeDescriptor.Match(container) && typeDescriptor.TryCollect(container, inner, this, out constructor))
            {
                return true;
            }
        }
        constructor = default!;
        return false;
    }

    public virtual void AddType(ITypeLookup descriptor) => _types.Add(descriptor);

    public virtual void AddParserLookup(IParserLookup parserLookup) => _parserLookups.Add(parserLookup);

    public virtual void AddModifier(IModifier modifier) => _modifiers.Add(modifier);

    public virtual void AddAdapter(IAdapterLookup adapterLookup) => _adapterLookups.Add(adapterLookup);
}