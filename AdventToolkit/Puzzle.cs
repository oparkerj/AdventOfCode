using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            if (puzzle.Part == 1) puzzle.PartOne();
            else puzzle.PartTwo();
        }

        public int Part { get; set; } = 1;

        public string InputName { get; set; }

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

        public void WriteLn(object o)
        {
            Console.WriteLine(o);
        }

        public void Write(object o)
        {
            Console.Write(o);
        }

        public void NewLine() => Console.WriteLine();

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