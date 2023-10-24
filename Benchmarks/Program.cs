using AdventToolkit.New;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks;

[MemoryDiagnoser]
public class BenchmarkMain
{
    public static void Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<BenchmarkMain>();
        // var summary = BenchmarkPuzzle<TestPuzzle>();
    }

    public static Summary BenchmarkPuzzle<T>()
        where T : new()
    {
        if (!GetGenericArguments(typeof(T), typeof(Puzzle<,>), out var args))
        {
            throw new ArgumentException($"Incompatible type {typeof(T).Name}");
        }

        var benchmark = typeof(PuzzleBenchmark<,,>).MakeGenericType(typeof(T), args[0], args[1]);
        return BenchmarkRunner.Run(benchmark);

        static Type GenericType(Type t) => t.IsGenericType ? t.GetGenericTypeDefinition() : t;

        static bool GetGenericArguments(Type input, Type target, out Type[] args)
        {
            while (true)
            {
                if (GenericType(input) == target)
                {
                    args = input.GetGenericArguments();
                    return true;
                }
                if (input.BaseType is not (not null and var baseType))
                {
                    args = default!;
                    return false;
                }
                input = baseType;
            }
        }
    }
}

[MemoryDiagnoser]
public class PuzzleBenchmark<TPuzzle, T1, T2>
    where TPuzzle : Puzzle<T1, T2>, new()
    where T2 : notnull
    where T1 : notnull
{
    private TPuzzle _puzzle = default!;

    [GlobalSetup(Target = nameof(PartOne))]
    public void SetupOne()
    {
        _puzzle = new TPuzzle {Part = 1};
        _puzzle.Input = _puzzle.GetInput();
    }
    
    [GlobalSetup(Target = nameof(PartTwo))]
    public void SetupTwo()
    {
        _puzzle = new TPuzzle {Part = 2};
        _puzzle.Input = _puzzle.GetInput();
    }

    [Benchmark]
    public T1 PartOne() => _puzzle.PartOne();

    [Benchmark]
    public T2 PartTwo() => _puzzle.PartTwo();
}