using System.Collections.Generic;

namespace AdventOfCode2019.IntCode;

public class OutputTools
{
    private Computer Source;

    public OutputTools(Computer source)
    {
        Source = source;
    }

    public IEnumerable<long> TakeCount(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return Source.NextOutput();
        }
    }
        
    public IEnumerable<int> TakeCountInt(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return Source.NextInt();
        }
    }
        
    public IEnumerable<bool> TakeCountBool(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            yield return Source.NextBool();
        }
    }
}