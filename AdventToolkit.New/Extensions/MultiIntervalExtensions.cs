using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Extensions;

public static class MultiIntervalExtensions
{
    /// <summary>
    /// Add multiple intervals.
    /// </summary>
    /// <param name="multiInterval"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    public static void Add<T>(this MultiInterval<T> multiInterval, IEnumerable<Interval<T>> other)
        where T : INumber<T>
    {
        foreach (var interval in other)
        {
            multiInterval.Add(interval);
        }
    }
    
    /// <summary>
    /// Remove multiple intervals.
    /// </summary>
    /// <param name="multiInterval"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    public static void Remove<T>(this MultiInterval<T> multiInterval, IEnumerable<Interval<T>> other)
        where T : INumber<T>
    {
        foreach (var interval in other)
        {
            multiInterval.Remove(interval);
        }
    }
}