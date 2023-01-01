using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;
using TextCopy;

namespace AdventToolkit;

public abstract class PuzzleBase
{
    protected PuzzleBase()
    {
        InputName = CopyInput(GetType());
    }

    public string CopyInput<T>() => CopyInput(typeof(T));

    public string CopyInput(Type type) => type.Name + ".txt";

    public void Run() => Run(this);

    public static void Run<T>()
        where T : PuzzleBase, new() => Run(new T());

    private static void Run<T>(T puzzle)
        where T : PuzzleBase
    {
        if (puzzle is Puzzle p) RunPuzzle(p);
        else if (puzzle.IsGenericType(typeof(Puzzle<>)))
        {
            RunGenericMethod<T>(puzzle, typeof(Puzzle<>), nameof(RunPuzzle1));
        }
        else if (puzzle.IsGenericType(typeof(Puzzle<,>)))
        {
            RunGenericMethod<T>(puzzle, typeof(Puzzle<,>), nameof(RunPuzzle2));
        }
    }

    private static void RunGenericMethod<T>(object puzzle, Type type, string method)
    {
        var info = typeof(PuzzleBase).GetMethod(method, BindingFlags.Public | BindingFlags.Static)!;
        var args = puzzle.GetGenericArguments(type).Prepend(typeof(T)).ToArray(info.GetGenericArguments().Length);
        info.MakeGenericMethod(args).Invoke(null, new[] {puzzle});
    }

    private void ClipLast(object value)
    {
        var str = value.ToString();
        Clip(str?.Split('\n')[^1].Trim() ?? "null");
    }

    public static void RunPuzzle<T>(T puzzle)
        where T : Puzzle
    {
        Action part = puzzle.Part == 1 ? puzzle.PartOne : puzzle.PartTwo;
        var capture = puzzle.GetType().HasAttribute<CopyResultAttribute>();
        Action<object> output = capture ? puzzle.ClipLast : puzzle.WriteLn;

        if (!puzzle.Measure)
        {
            output(RunCapture(part));
        }
        else if (capture)
        {
            var watch = new Stopwatch();
            watch.Start();
            var console = RunCapture(part);
            watch.Stop();
            puzzle.ClipLast(console);
            Console.WriteLine(watch.Elapsed);
        }
        else
        {
            var watch = new Stopwatch();
            watch.Start();
            part();
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }
    }
    
    public static void RunPuzzle1<T, TR>(T puzzle)
        where T : Puzzle<TR>
    {
        Action<object> output;
        if (puzzle.GetType().HasAttribute<CopyResultAttribute>()) output = puzzle.Clip;
        else output = puzzle.WriteLn;
        
        if (!puzzle.Measure)
        {
            output(puzzle.Part == 1 ? puzzle.PartOne() : puzzle.PartTwo());
        }
        else
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = puzzle.Part == 1 ? puzzle.PartOne() : puzzle.PartTwo();
            watch.Stop();
            output(result);
            Console.WriteLine(watch.Elapsed);
        }
    }
    
    public static void RunPuzzle2<T, T1, T2>(T puzzle)
        where T : Puzzle<T1, T2>
    {
        Action<object> output;
        if (puzzle.GetType().HasAttribute<CopyResultAttribute>()) output = puzzle.Clip;
        else output = puzzle.WriteLn;
        
        if (!puzzle.Measure)
        {
            if (puzzle.Part == 1) output(puzzle.PartOne());
            else output(puzzle.PartTwo());
        }
        else
        {
            var watch = new Stopwatch();
            if (puzzle.Part == 1)
            {
                watch.Start();
                var result = puzzle.PartOne();
                watch.Stop();
                output(result);
            }
            else
            {
                watch.Start();
                var result = puzzle.PartTwo();
                watch.Stop();
                output(result);
            }
            Console.WriteLine(watch.Elapsed);
        }
    }

    public static string RunCapture<T>()
        where T : PuzzleBase, new()
    {
        return RunCapture(Run<T>);
    }

    public static string RunCapture(Action action)
    {
        var old = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);
        action();
        Console.SetOut(old);
        return output.ToString();
    }

    public int Part { get; set; } = 2;

    public string InputName { get; set; }

    public bool Measure { get; set; }

    // Execute the provided action with no console output
    public void Run(Action action)
    {
        RunCapture(action);
    }

    public void Run<T>(Func<T> func)
    {
        Run((Action) (() => func()));
    }

    public void WriteLn() => Console.WriteLine();

    public void WriteLn(object o) => Console.WriteLine(o);

    public void WriteLn(string s) => Console.WriteLine(s);

    public void Write(object o) => Console.Write(o);

    public void Write(string s) => Console.Write(s);

    public void NewLine() => Console.WriteLine();

    public void Clip(object o) => Clip(o.ToString());

    public void Clip(string text)
    {
        ClipboardService.SetText(text);
        WriteLn(text);
    }

    public void PrintLn(string text) => Print(text + Environment.NewLine);

    public void Print(string text)
    {
        Write(text);
        var name = InputName;
        if (name.IndexOf('.') is >= 0 and var i) name = name[..i] + "_out" + name[i..];
        File.AppendAllText(name, text);
    }

    private string[] _input;

    public string[] Input
    {
        get
        {
            if (_input != null) return _input;
            _input = File.ReadAllLines(InputName);
            return _input;
        }
        set => _input = value;
    }

    public string InputLine => Input[0];

    public int InputInt => InputLine.AsInt();

    private string[][] _groups;

    public string[][] AllGroups
    {
        get
        {
            if (_groups != null) return _groups;
            return _groups = Groups.ToArray();
        }
    }

    public IEnumerable<string[]> Groups
    {
        get
        {
            var last = 0;
            var current = 0;
            foreach (var s in Input)
            {
                if (string.IsNullOrEmpty(s))
                {
                    if (last != current) yield return Input[last..current];
                    last = current + 1;
                }
                current++;
            }
            if (last < current) yield return Input[last..current];
        }
    }
}

public abstract class Puzzle : PuzzleBase
{
    public abstract void PartOne();

    public virtual void PartTwo() => PartOne();
}

public abstract class Puzzle<T> : PuzzleBase
{
    public abstract T PartOne();

    public virtual T PartTwo() => PartOne();
}

public abstract class Puzzle<T1, T2> : PuzzleBase
{
    public abstract T1 PartOne();

    public abstract T2 PartTwo();
}

public abstract class Improve<TDay> : Puzzle
{
    protected Improve() => InputName = CopyInput<TDay>();
}

public abstract class Improve<TDay, T> : Puzzle<T>
{
    protected Improve() => InputName = CopyInput<TDay>();
}

public abstract class Improve<TDay, T1, T2> : Puzzle<T1, T2>
{
    protected Improve() => InputName = CopyInput<TDay>();
}