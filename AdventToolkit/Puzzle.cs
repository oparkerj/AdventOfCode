using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AdventToolkit.Extensions;
using TextCopy;

namespace AdventToolkit;

public abstract class PuzzleBase
{
    protected PuzzleBase()
    {
        InputName = GetType().Name + ".txt";
    }

    public static void Run<T>()
        where T : PuzzleBase, new()
    {
        var puzzle = new T();
        if (puzzle is Puzzle p) RunPuzzle(p);
        else if (puzzle.IsGenericType(typeof(Puzzle<>)))
        {
            var info = typeof(PuzzleBase).GetMethod(nameof(RunPuzzle1), BindingFlags.Public | BindingFlags.Static);
            var args = puzzle.GetGenericArguments(typeof(Puzzle<>)).Prepend(typeof(T)).ToArray();
            info!.MakeGenericMethod(args).Invoke(null, new object[] {puzzle});
        }
        else if (puzzle.IsGenericType(typeof(Puzzle<,>)))
        {
            var info = typeof(PuzzleBase).GetMethod(nameof(RunPuzzle2), BindingFlags.Public | BindingFlags.Static);
            var args = puzzle.GetGenericArguments(typeof(Puzzle<,>)).Prepend(typeof(T)).ToArray();
            info!.MakeGenericMethod(args).Invoke(null, new object[] {puzzle});
        }
    }

    public static void RunPuzzle<T>(T puzzle)
        where T : Puzzle
    {
        if (!puzzle.Measure)
        {
            if (puzzle.Part == 1) puzzle.PartOne();
            else puzzle.PartTwo();
        }
        else
        {
            var watch = new Stopwatch();
            watch.Start();
            if (puzzle.Part == 1) puzzle.PartOne();
            else puzzle.PartTwo();
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }
    }
    
    public static void RunPuzzle1<T, TR>(T puzzle)
        where T : Puzzle<TR>
    {
        if (!puzzle.Measure)
        {
            if (puzzle.Part == 1) puzzle.WriteLn(puzzle.PartOne());
            else puzzle.WriteLn(puzzle.PartTwo());
        }
        else
        {
            TR result;
            var watch = new Stopwatch();
            watch.Start();
            if (puzzle.Part == 1) result = puzzle.PartOne();
            else result = puzzle.PartTwo();
            watch.Stop();
            puzzle.WriteLn(result);
            Console.WriteLine(watch.Elapsed);
        }
    }
    
    public static void RunPuzzle2<T, T1, T2>(T puzzle)
        where T : Puzzle<T1, T2>
    {
        if (!puzzle.Measure)
        {
            if (puzzle.Part == 1) puzzle.WriteLn(puzzle.PartOne());
            else puzzle.WriteLn(puzzle.PartTwo());
        }
        else
        {
            var watch = new Stopwatch();
            if (puzzle.Part == 1)
            {
                T1 result;
                watch.Start();
                result = puzzle.PartOne();
                watch.Stop();
                puzzle.WriteLn(result);
            }
            else
            {
                T2 result;
                watch.Start();
                result = puzzle.PartTwo();
                watch.Stop();
                puzzle.WriteLn(result);
            }
            Console.WriteLine(watch.Elapsed);
        }
    }

    public static string RunCapture<T>()
        where T : PuzzleBase, new()
    {
        var old = Console.Out;
        using var output = new StringWriter();
        Console.SetOut(output);
        Run<T>();
        Console.SetOut(old);
        return output.ToString();
    }

    public int Part { get; set; } = 2;

    public string InputName { get; set; }

    public bool Measure { get; set; }

    // Execute the provided action with no console output
    public void Run(Action action)
    {
        var old = Console.Out;
        Console.SetOut(TextWriter.Null);
        action();
        Console.SetOut(old);
    }

    public void Run<T>(Func<T> func)
    {
        Run((Action) (() => func()));
    }

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