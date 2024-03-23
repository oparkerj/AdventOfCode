using System.Diagnostics;
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
    private readonly List<ITypeDescriptor> _types = [];
    private readonly List<IParserLookup> _parserLookups = [];
    private readonly List<IModifier> _modifiers = [];
    private readonly List<IAdapterLookup> _adapterLookups = [];

    private Stack<DisambiguationSection>? _disambiguation;
    private bool _disambiguationComplete;

    public IEnumerable<ITypeDescriptor> Types => _types;
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
        if (!(_disambiguation?.Count > 0)) return;

        while (_disambiguation.Peek() is {Current: var current} && current.IsTupleType())
        {
            _disambiguation.Push(new DisambiguationSection(current.GetGenericArguments()));
        }

        _disambiguationComplete = _disambiguation.Peek().Current == typeof(Null);
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
        // Skip if no state or all sections used
        if (_disambiguation?.Count is null or 0) return type is null;

        var current = _disambiguation.Peek();

        // Null type will advance to the next section
        if (type is null)
        {
            if (!_disambiguationComplete)
            {
                throw new ArgumentException("Disambiguation was not fully applied when advancing state.");
            }

            // Find the next type
            while (true)
            {
                _disambiguation.Pop();
                // If the index reaches the end, pop the stack
                if (current.Index + 1 >= current.Parts.Length)
                {
                    // Keep going until we find a section to advance
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
        var match = (type.IsGenericTypeDefinition && current.Current.TryGetTypeArguments(type, out _)) || current.Current == type;
        if (!match) return false;
        
        Debug.Assert(current.Current.IsAssignableTo(typeof(IDisambiguation)));
        var next = (Type?) current.Current.GetMethod(nameof(IDisambiguation.Apply), [typeof(Type)])!.Invoke(null, [current.Current]) ?? typeof(Null);
        current.Current = next;
        _disambiguationComplete = next == typeof(Null);
        return true;
    }

    public virtual bool TryLookupType(Type type, out ITypeDescriptor descriptor)
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

    public virtual void AddType(ITypeDescriptor descriptor) => _types.Add(descriptor);

    public virtual void AddParserLookup(IParserLookup parserLookup) => _parserLookups.Add(parserLookup);

    public virtual void AddModifier(IModifier modifier) => _modifiers.Add(modifier);

    public virtual void AddAdapter(IAdapterLookup adapterLookup) => _adapterLookups.Add(adapterLookup);
}