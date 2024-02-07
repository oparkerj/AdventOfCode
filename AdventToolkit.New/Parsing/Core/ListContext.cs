using AdventToolkit.New.Parsing.Interface;

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

    public IEnumerable<ITypeDescriptor> Types => _types;
    public IEnumerable<IParserLookup> ParserLookups => _parserLookups;
    public IEnumerable<IModifier> Modifiers => _modifiers;
    public IEnumerable<IAdapterLookup> AdapterLookups => _adapterLookups;

    public virtual bool TryLookupType(Type type, out ITypeDescriptor descriptor)
    {
        foreach (var typeDescriptor in _types)
        {
            if (!typeDescriptor.Match(type)) continue;
            descriptor = typeDescriptor;
            return true;
        }
        descriptor = default;
        return false;
    }

    public virtual bool TryLookupParser<T>(Type inputType, T value, string extra, out IParser parser)
    {
        foreach (var parserLookup in _parserLookups)
        {
            if (parserLookup.TryLookup(inputType, value, extra, this, out parser)) return true;
        }
        parser = default;
        return false;
    }

    public virtual bool TryLookupAdapter(Type from, Type to, out IParser parser)
    {
        foreach (var adapterLookup in _adapterLookups)
        {
            if (adapterLookup.TryLookup(from, to, this, out parser)) return true;
        }
        parser = default;
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