using AdventToolkit2.Parsing.Context;
using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Space;
using AdventToolkit2.Space.Set;

namespace AdventToolkit2.Parsing.Core;

/// <summary>
/// The default context if none is specified.
/// </summary>
public class DefaultContext : ListContext
{
    private static DefaultContext? _instance;

    public static DefaultContext Instance => _instance ??= new DefaultContext();

    /// <summary>
    /// Add definitions for common types.
    /// </summary>
    /// <param name="context"></param>
    public static void AddCommonTypes(IParseContext? context = null)
    {
        context ??= Instance;
        
        context.AddParserLookup(new LiteralParser());
        
        context.AddParserLookup(new JoinToString());
        
        var stringParse = new StringParse();
        context.AddParserLookup(stringParse);
        context.AddAdapter(stringParse);
        context.AddType(stringParse);
        
        context.AddAdapter(new CharParse());
        
        context.AddAdapter(new StringAdapter());

        context.AddParserLookup(new TypeParse());
        
        context.AddParserLookup(new DictParse());
        
        context.AddParserLookup(new EnumerableActions());
        
        context.AddLookup<ListParse>();
        
        context.AddLookup<TupleParse>();
        
        context.AddLookup<ArrayParse>();
        
        var regexParse = new RegexParse();
        context.AddParserLookup(regexParse);
    }

    public static void AddToolkitTypes(IParseContext? context = null)
    {
        context ??= Instance;
        
        context.AddType<Pos.Descriptor>();
        context.AddType<Pos3.Descriptor>();
        context.AddType<Pos4.Descriptor>();
        context.AddAdapter(new Grid<byte>());
    }
}