using AdventToolkit.New.Parsing.Context;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Space;
using AdventToolkit.New.Space.Set;

namespace AdventToolkit.New.Parsing.Core;

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
        
        var stringParse = new StringParse();
        context.AddParserLookup(stringParse);
        context.AddAdapter(stringParse);
        context.AddType(stringParse);
        
        context.AddAdapter(new StringAdapter());

        context.AddParserLookup(new TypeParse());
        
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
        context.AddAdapter(new Grid<byte>());
    }
}