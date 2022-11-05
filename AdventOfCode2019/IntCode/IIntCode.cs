using System;
using System.Threading.Tasks;
using AdventToolkit.Utilities.Threads;

namespace AdventOfCode2019.IntCode;

public interface IIntCode
{
    Lock Interrupt { get; }
    
    Func<long> Input { get; set; }
    
    Action<long> Output { get; set; }

    void Execute();
}

public static class IntCodeExtensions
{
    public static Task ExecuteAsync(this IIntCode i)
    {
        return Task.Run(i.Execute);
    }
}