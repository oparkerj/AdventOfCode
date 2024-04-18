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

        context.AddAdapter(new CharParse());

        context.AddParserLookup(new TypeParse());
        
        context.AddType(new ListParse());
        
        context.AddType(new TupleParse());
        
        context.AddType(new ArrayParse());
    }

    public static void AddToolkitTypes(IParseContext? context = null)
    {
        context ??= Instance;
        
        context.AddType(new Pos<byte>());
        context.AddAdapter(new Grid<byte>());
    }
}