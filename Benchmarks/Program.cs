using BenchmarkDotNet.Running;

namespace Benchmarks;

public class Testing
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Testing>();
    }
}