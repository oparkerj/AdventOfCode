using System.Text;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing;

public class ParseGraph
{
    private HashSet<string> _labels = [];
    private List<(string, string)> _edges = [];
    private int _id;

    private string TypeName(Type type)
    {
        var result = new StringBuilder();

        var name = type.Name;
        if (type.IsGenericType)
        {
            name = name[..name.IndexOf('`')];
        }
        result.Append(name);
        
        if (type.IsGenericType)
        {
            var inner = string.Join(", ", type.GetGenericArguments().Select(TypeName));
            result.Append('<').Append(inner).Append('>');
        }

        return result.ToString();
    }

    public string Add(IParser parser)
    {
        var name = TypeName(parser.GetType());
        var label = $"{_id++}: {name}";
        _labels.Add(label);
        
        foreach (var child in parser.GetChildren())
        {
            var childLabel = Add(child);
            _edges.Add((label, childLabel));
        }

        return label;
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append("digraph G {\n");
        
        foreach (var label in _labels)
        {
            result.Append($"\"{label}\"\n");
        }
        
        foreach (var (from, to) in _edges)
        {
            result.Append($"\"{from}\" -> \"{to}\"\n");
        }

        result.Append('}');
        return result.ToString();
    }
}