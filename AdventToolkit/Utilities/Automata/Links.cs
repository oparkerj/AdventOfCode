using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities.Automata;

public class Links<T> : IEnumerable<Link<T>>
{
    public readonly List<Link<T>> Possible = new();

    public int Count => Possible.Count;

    private Links<T> CopiedLinks()
    {
        var links = new Links<T>();
        foreach (var link in Copies())
        {
            links.Possible.Add(link);
        }
        return links;
    }
        
    private IEnumerable<Link<T>> Copies() => Possible.Select(link => link.ShallowCopy());

    public void Add(Link<T> link, bool lazy = false)
    {
        if (lazy) Possible.Insert(Possible.Count - 1, link);
        else Possible.Add(link);
    }

    public void AddRange(IEnumerable<Link<T>> links, bool lazy = false)
    {
        foreach (var link in links)
        {
            Add(link, lazy);
        }
    }

    public Links<T> Offset(int offset)
    {
        var links = CopiedLinks();
        foreach (var link in links)
        {
            link.To += offset;
        }
        return links;
    }

    public Links<T> Consume(bool consume)
    {
        var links = CopiedLinks();
        foreach (var link in links)
        {
            link.Consume = consume;
        }
        return links;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<Link<T>> GetEnumerator()
    {
        return Possible.GetEnumerator();
    }
}