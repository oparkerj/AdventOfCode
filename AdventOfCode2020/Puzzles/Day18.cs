using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day18 : Puzzle
    {
        public Day18()
        {
            Part = 2;
        }

        public int GetEnd(string s, int start)
        {
            var level = 0;
            for (var i = start; i < s.Length; i++)
            {
                if (s[i] == '(') level++;
                if (s[i] == ')') level--;
                if (level == 0) return i;
            }
            return -1;
        }

        public IEnumerable<string> ExpressionSplit(string s)
        {
            var b = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (b.Length > 0) yield return b.ToString();
                    b.Length = 0;
                }
                else if (s[i] == '(')
                {
                    var end = GetEnd(s, i);
                    yield return s[(i + 1)..end];
                    i = end;
                    b.Length = 0;
                }
                else b.Append(s[i]);
            }
            if (b.Length > 0) yield return b.ToString();
        }

        public long Eval(string s)
        {
            var parts = ExpressionSplit(s).ToArray();
            if (parts.Length == 1)
            {
                if (!parts[0].Contains(' ')) return long.Parse(parts[0]);
                return Eval(parts[0]);
            }
            var current = Eval(parts[0]);
            for (var i = 1; i < parts.Length; i += 2)
            {
                if (parts[i] == "+") current += Eval(parts[i + 1]);
                else if (parts[i] == "*") current *= Eval(parts[i + 1]);
            }
            return current;
        }

        public override void PartOne()
        {
            var sum = Input.Select(Eval).Sum();
            WriteLn(sum);
        }
        
        public long Eval2(string s)
        {
            var parts = ExpressionSplit(s).ToArray();
            if (parts.Length == 1)
            {
                if (!parts[0].Contains(' ')) return long.Parse(parts[0]);
                return Eval(parts[0]);
            }

            while (parts.Search("+", out var i))
            {
                parts = parts[..(i - 1)].Append((Eval2(parts[i - 1]) + Eval2(parts[i + 1])).ToString())
                    .Concat(parts[(i + 2)..])
                    .ToArray();
            }
            while (parts.Search("*", out var i))
            {
                parts = parts[..(i - 1)].Append((Eval2(parts[i - 1]) * Eval2(parts[i + 1])).ToString())
                    .Concat(parts[(i + 2)..])
                    .ToArray();
            }

            return long.Parse(parts[0]);
        }

        public override void PartTwo()
        {
            var sum = Input.Select(Eval2).Sum();
            WriteLn(sum);
        }
    }
}