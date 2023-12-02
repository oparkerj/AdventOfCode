using System;
using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class DebugExtensions
{
    public static IEnumerable<T> DebugStage<T>(this IEnumerable<T> input, DebugContext context)
    {
        return context.CreateStage(input);
    }

    public static T DebugValue<T>(this T value, DebugContext context)
    {
        return context.Add(value);
    }
}

public class DebugContext : IDisposable
{
    private List<List<string>> _stages = new();
    
    public string Name { get; }

    public DebugContext(string name) => Name = name;

    private string GetString<T>(T t) => t?.ToString() ?? string.Empty;

    public IEnumerable<T> CaptureStage<T>(IEnumerable<T> input, int stage)
    {
        foreach (var t in input)
        {
            _stages[stage].Add(GetString(t));
            yield return t;
        }
    }

    public IEnumerable<T> CreateStage<T>(IEnumerable<T> input)
    {
        var stage = _stages.Count;
        _stages.Add(new List<string>());
        return CaptureStage(input, stage);
    }

    public T Add<T>(T t)
    {
        _stages.Add(new List<string>(1) {GetString(t)});
        return t;
    }
        
    public void Dispose()
    {
        Console.WriteLine($"Debug {Name}:");
        for (var i = 0; i < _stages.Count; i++)
        {
            Console.WriteLine($"Stage {i}: {string.Join(", ", _stages[i])}");
        }
    }
}