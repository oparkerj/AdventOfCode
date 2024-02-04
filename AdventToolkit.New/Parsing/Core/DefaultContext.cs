using AdventToolkit.New.Parsing.Context;

namespace AdventToolkit.New.Parsing.Core;

public class DefaultContext : ListContext
{
    private static DefaultContext? _instance;

    public static DefaultContext Instance => _instance ??= new DefaultContext();

    public DefaultContext()
    {
        AddDefaults();
    }

    public void AddDefaults()
    {
        var stringParse = new StringParse();
        AddParserLookup(stringParse);
        AddAdapter(stringParse);
        AddType(stringParse);

        var listParse = new ListParse();
        AddType(listParse);
    }
}