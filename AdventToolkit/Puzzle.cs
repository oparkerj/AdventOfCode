using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
                        yield return Input[last..current];
                        last = current + 1;
                    }
                    current++;
                }
                if (last < current) yield return Input[last..current];
            }
        }

        public bool Range(int i, int lower, int upper)
        {
            return i >= lower && i < upper;
        }

        public bool Range(string s, int lower, int upper)
        {
            return Range(int.Parse(s), lower, upper);
        }

        public bool MatchAll(string s, string regex)
        {
            return Regex.IsMatch(s, "^" + regex + "$");
        }
    }
}