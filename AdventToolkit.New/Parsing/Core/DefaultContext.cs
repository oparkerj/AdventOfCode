using AdventToolkit.New.Parsing.Context;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// The default context if none is specified.
/// </summary>
public class DefaultContext : ListContext
{
    private static DefaultContext? _instance;

    public static DefaultContext Instance => _instance ??= new DefaultContext();

    public DefaultContext() => AddDefaults();

    /// <summary>
    /// Add builtin conversions and types
    /// </summary>
    public void AddDefaults()
    {
        var stringParse = new StringParse();
        AddParserLookup(stringParse);
        AddAdapter(stringParse);
        AddType(stringParse);

        var listParse = new ListParse();
        AddType(listParse);

        var tupleParse = new TupleParse();
        AddType(tupleParse);

        var arrayParse = new ArrayParse();
        AddType(arrayParse);

        var typeParse = new TypeParse();
        AddParserLookup(typeParse);
    }
}