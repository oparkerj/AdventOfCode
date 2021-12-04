using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventToolkit.Extensions;
using TextCopy;

namespace AdventToolkit
{
    public abstract class Puzzle
    {
        protected Puzzle()
        {
            InputName = GetType().Name + ".txt";
        }

        public static void Run<T>()
            where T : Puzzle, new()
        {
            var puzzle = new T();
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

        public static string RunCapture<T>()
            where T : Puzzle, new()
        {
            var old = Console.Out;
            using var output = new StringWriter();
            Console.SetOut(output);
            Run<T>();
            Console.SetOut(old);
            return output.ToString();
        }

        public int Part { get; set; } = 1;

        public string InputName { get; set; }

        public bool Measure { get; set; }

        public abstract void PartOne();

        public virtual void PartTwo() => PartOne();

        // Execute the provided action with no console output
        public void Run(Action action)
        {
            var old = Console.Out;
            Console.SetOut(TextWriter.Null);
            action();
            Console.SetOut(old);
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
                    if (s == "")
                    {
                        if (last != current) yield return Input[last..current];
                        last = current + 1;
                    }
                    current++;
                }
                if (last < current) yield return Input[last..current];
            }
        }

        public bool InRange(int i, int lower, int upper, bool inclusive = false)
        {
            if (inclusive) return i >= lower && i <= upper;
            return i >= lower && i < upper;
        }

        public bool InRange(string s, int lower, int upper)
        {
            return InRange(int.Parse(s), lower, upper);
        }
    }
}