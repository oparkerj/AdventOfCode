using AdventOfCode2023.Puzzles;
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
        var summary = ComparePuzzle<Day1, Day1Better>();
    }
    
    private static Type GenericType(Type t) => t.IsGenericType ? t.GetGenericTypeDefinition() : t;

    private static bool GetGenericArguments(Type input, Type target, out Type[] args)
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

    public static Summary BenchmarkPuzzle<T>()
        where T : new()
    {
        if (!GetGenericArguments(typeof(T), typeof(Puzzle<,>), out var args))
        {
            throw new ArgumentException($"Incompatible type {typeof(T).Name}");
        }

        var benchmark = typeof(PuzzleBenchmark<,,>).MakeGenericType(typeof(T), args[0], args[1]);
        return BenchmarkRunner.Run(benchmark);
    }

    public static Summary ComparePuzzle<T1, T2>()
        where T1 : new()
        where T2 : new()
    {
        if (!GetGenericArguments(typeof(T1), typeof(Puzzle<,>), out var args1))
        {
            throw new ArgumentException($"Incompatible type {typeof(T1).Name}");
        }
        if (!GetGenericArguments(typeof(T2), typeof(Puzzle<,>), out var args2))
        {
            throw new ArgumentException($"Incompatible type {typeof(T2).Name}");
        }
        if (!args1.SequenceEqual(args2))
        {
            throw new ArgumentException("Puzzles do not have the same types.");
        }

        var benchmark = typeof(PuzzleCompare<,,,>).MakeGenericType(typeof(T1), typeof(T2), args1[0], args1[1]);
        return BenchmarkRunner.Run(benchmark);
    }
}

[MemoryDiagnoser]
public class PuzzleBenchmark<TPuzzle, T1, T2>
    where TPuzzle : Puzzle<T1, T2>, new()
    where T2 : notnull
    where T1 : notnull
{
    public string InputDir = Environment.GetEnvironmentVariable("AOC_INPUT") ?? string.Empty;
    
    private TPuzzle _puzzle = default!;

    [GlobalSetup(Target = nameof(PartOne))]
    public void SetupOne()
    {
        _puzzle = new TPuzzle {Part = 1};
        _puzzle.InputDirectory = InputDir;
        _puzzle.Input = _puzzle.GetLines();
    }
    
    [GlobalSetup(Target = nameof(PartTwo))]
    public void SetupTwo()
    {
        _puzzle = new TPuzzle {Part = 2};
        _puzzle.InputDirectory = InputDir;
        _puzzle.Input = _puzzle.GetLines();
    }

    [Benchmark]
    public T1 PartOne() => _puzzle.PartOne();

    [Benchmark]
    public T2 PartTwo() => _puzzle.PartTwo();
}

[MemoryDiagnoser]
public class PuzzleCompare<TPuzzle1, TPuzzle2, T1, T2>
    where TPuzzle1 : Puzzle<T1, T2>, new()
    where TPuzzle2 : Puzzle<T1, T2>, new()
    where T1 : notnull
    where T2 : notnull
{
    public string InputDir = Environment.GetEnvironmentVariable("AOC_INPUT") ?? string.Empty;
    
    private TPuzzle1 _puzzle1 = default!;
    private TPuzzle2 _puzzle2 = default!;

    [GlobalSetup(Targets = new[] {nameof(LeftPartOne), nameof(RightPartOne)})]
    public void SetupOne()
    {
        _puzzle1 = new TPuzzle1 {Part = 1};
        _puzzle1.InputDirectory = InputDir;
        _puzzle1.Input = _puzzle1.GetLines();
        _puzzle2 = new TPuzzle2 {Part = 1};
        _puzzle2.InputDirectory = InputDir;
        _puzzle2.Input = _puzzle2.GetLines();
    }
    
    [GlobalSetup(Targets = new[] {nameof(LeftPartTwo), nameof(RightPartTwo)})]
    public void SetupTwo()
    {
        _puzzle1 = new TPuzzle1 {Part = 2};
        _puzzle1.InputDirectory = InputDir;
        _puzzle1.Input = _puzzle1.GetLines();
        _puzzle2 = new TPuzzle2 {Part = 2};
        _puzzle2.InputDirectory = InputDir;
        _puzzle2.Input = _puzzle2.GetLines();
    }

    [Benchmark]
    public T1 LeftPartOne() => _puzzle1.PartOne();

    [Benchmark]
    public T2 LeftPartTwo() => _puzzle1.PartTwo();
    
    [Benchmark]
    public T1 RightPartOne() => _puzzle2.PartOne();

    [Benchmark]
    public T2 RightPartTwo() => _puzzle2.PartTwo();
}