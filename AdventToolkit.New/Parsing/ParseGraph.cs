using System.Text;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// This is a debugging class that can generate a DOT graph from a parser.
/// This class is the reason <see cref="IParser.GetChildren"/> should be implemented.
/// </summary>
public class ParseGraph
{
    private HashSet<string> _labels = [];
    private List<(string, string)> _edges = [];
    private int _id;

    /// <summary>
    /// Get the display name for a type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Add a parser to the graph.
    /// </summary>
    /// <param name="parser"></param>
    /// <returns></returns>
    public string Add(IParser parser)
    {
        var name = TypeName(parser.GetType());
        // An ID number is added so multiple of the same parser type can
        // appear in the graph.
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